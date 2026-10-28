using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Okno intonacie skupin jazyka - sposob (bez kontextu, kontext, nosna veta), text pred a za uryvkom a interpunkcia.
/// Predvolene hodnoty vychadzaju z typu skupiny; ulozia sa len odlisne (<see cref="Overrides" />).
/// </summary>
public partial class FProsody : Form
{
    private readonly BindingList<ProsodyRow> _rows;
    private readonly Func<GroupProsody, FyzGroup, CancellationToken, Task<GeneratedSound>>? _try;
    private readonly AudioPlayer _player;
    private readonly CancellationTokenSource _cancel = new();

    /// <param name="groups">skupiny jazyka</param>
    /// <param name="saved">ulozene nastavenia podla kluca skupiny</param>
    /// <param name="defaults">predvolena intonacia skupiny</param>
    /// <param name="tryGroup">vygeneruje ukazku prveho zvuku skupiny; <see langword="null" />, ak hlas jazyka nie je nastaveny</param>
    /// <param name="player">prehravac ukazky</param>
    public FProsody(IEnumerable<FyzGroup> groups, IReadOnlyDictionary<string, GroupProsody> saved, Func<FyzGroup, GroupProsody> defaults,
        Func<GroupProsody, FyzGroup, CancellationToken, Task<GeneratedSound>>? tryGroup, AudioPlayer player)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _try = tryGroup;
        _player = player;
        _rows = new BindingList<ProsodyRow>(groups
            .Select(g => new ProsodyRow(g, defaults(g), saved.TryGetValue(g.Key, out var prosody) ? prosody : null))
            .ToList());

        lHint.Text = Resources.FProsody_Hint;
        cTechnique.DataSource = new[]
        {
            new Choice<ProsodyTechnique>(ProsodyTechnique.None, Resources.Prosody_None),
            new Choice<ProsodyTechnique>(ProsodyTechnique.Context, Resources.Prosody_Context),
            new Choice<ProsodyTechnique>(ProsodyTechnique.Carrier, Resources.Prosody_Carrier)
        };
        cPunctuation.DataSource = new[]
        {
            new Choice<ProsodyPunctuation>(ProsodyPunctuation.None, Resources.Prosody_PunctuationNone),
            new Choice<ProsodyPunctuation>(ProsodyPunctuation.Comma, Resources.Prosody_PunctuationComma),
            new Choice<ProsodyPunctuation>(ProsodyPunctuation.Period, Resources.Prosody_PunctuationPeriod)
        };
        foreach (var column in new[] { cTechnique, cPunctuation })
        {
            column.ValueMember = nameof(Choice<ProsodyTechnique>.Value);
            column.DisplayMember = nameof(Choice<ProsodyTechnique>.Name);
        }

        dgvGroups.AutoGenerateColumns = false;
        dgvGroups.DataSource = _rows;
        bTry.Enabled = _try is not null;
        lStatus.Text = _try is null ? Resources.FProsody_NoVoice : "";
        FormClosed += (_, _) =>
        {
            _cancel.Cancel();
            _cancel.Dispose();
        };
    }

    /// <summary>
    /// Nastavenia skupin, ktore sa lisia od predvolenych (po OK).
    /// </summary>
    public Dictionary<string, GroupProsody> Overrides { get; private set; } = [];

    private IEnumerable<ProsodyRow> SelectedRows =>
        dgvGroups.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<ProsodyRow>();

    private void BDefault_Click(object? sender, EventArgs e)
    {
        foreach (var row in SelectedRows)
            row.Load(row.Default);
        _rows.ResetBindings();
    }

    private async void BTry_Click(object? sender, EventArgs e)
    {
        if (_try is null || dgvGroups.CurrentRow?.DataBoundItem is not ProsodyRow row)
            return;

        dgvGroups.EndEdit();
        bTry.Enabled = false;
        lStatus.Text = Resources.FVoiceProfile_Generating;
        try
        {
            var result = await _try(row.ToProsody(), row.Group, _cancel.Token);
            if (IsDisposed)
                return;

            _player.Play(SoundProcessing.ToWav(result.Sound));
            lStatus.Text = result.FromCache
                ? Resources.FVoiceProfile_SampleCached
                : string.Format(CultureInfo.CurrentCulture, Resources.FVoiceProfile_SampleDone, result.Characters);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex) when (ex is SpeechException or InvalidDataException or ArgumentException)
        {
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        if (!IsDisposed)
            bTry.Enabled = true;
    }

    private void DgvGroups_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        // vyber v rozbalovacom zozname sa prejavi hned
        if (dgvGroups.CurrentCell is DataGridViewComboBoxCell)
            dgvGroups.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void DgvGroups_DataError(object? sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

    private void BOK_Click(object? sender, EventArgs e)
    {
        dgvGroups.EndEdit();
        Overrides = _rows.Where(r => !r.ToProsody().SameAs(r.Default))
            .ToDictionary(r => r.GroupKey, r => r.ToProsody(), StringComparer.OrdinalIgnoreCase);
        DialogResult = DialogResult.OK;
    }

    /// <summary>
    /// Polozka rozbalovacieho zoznamu v mriezke.
    /// </summary>
    private sealed record Choice<T>(T Value, string Name);
}

/// <summary>
/// Riadok okna intonacie - skupina a jej nastavenie.
/// </summary>
public sealed class ProsodyRow
{
    /// <param name="group">skupina</param>
    /// <param name="defaults">predvolena intonacia skupiny</param>
    /// <param name="saved">ulozena intonacia; <see langword="null" /> = predvolena</param>
    public ProsodyRow(FyzGroup group, GroupProsody defaults, GroupProsody? saved)
    {
        Group = group;
        Default = defaults;
        Load(saved ?? defaults);
    }

    /// <summary>Skupina.</summary>
    [Browsable(false)]
    public FyzGroup Group { get; }

    /// <summary>Predvolena intonacia.</summary>
    [Browsable(false)]
    public GroupProsody Default { get; }

    /// <summary>Kluc skupiny.</summary>
    public string GroupKey => Group.Key;

    /// <summary>Typ skupiny s popisom.</summary>
    public string TypeName => Group.Type.Description.Length == 0 ? Group.Type.Name : Group.Type.Description;

    /// <summary>Sposob.</summary>
    public ProsodyTechnique Technique { get; set; }

    /// <summary>Interpunkcia.</summary>
    public ProsodyPunctuation Punctuation { get; set; }

    /// <summary>Text pred uryvkom.</summary>
    public string Before { get; set; } = "";

    /// <summary>Text za uryvkom.</summary>
    public string After { get; set; } = "";

    /// <summary>
    /// Prevezme nastavenie.
    /// </summary>
    public void Load(GroupProsody prosody)
    {
        Technique = prosody.Technique;
        Punctuation = prosody.Punctuation;
        Before = prosody.Before;
        After = prosody.After;
    }

    /// <summary>
    /// Nastavenie riadku.
    /// </summary>
    public GroupProsody ToProsody() => new()
    {
        Technique = Technique,
        Punctuation = Punctuation,
        Before = Before?.Trim() ?? "",
        After = After?.Trim() ?? ""
    };
}
