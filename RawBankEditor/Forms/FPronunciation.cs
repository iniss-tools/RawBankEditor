using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Okno slovnika vyslovnosti jazyka - pravidla v poradi uplatnenia, cisla slovami a skuska na texte (vysledny text,
/// ukazka hlasom).
/// </summary>
public partial class FPronunciation : Form
{
    private readonly BindingList<PronunciationRule> _rules;
    private readonly Func<string, IReadOnlyList<PronunciationRule>, bool, CancellationToken, Task<GeneratedSound>>? _try;
    private readonly string _languageCode;
    private readonly AudioPlayer _player;
    private readonly CancellationTokenSource _cancel = new();

    /// <param name="rules">ulozene pravidla</param>
    /// <param name="numbersAsWords">cisla citat slovami</param>
    /// <param name="languageCode">jazyk reci (ISO 639-1) - prevod cisel je len pre slovencinu a cestinu</param>
    /// <param name="sampleText">skusobny text</param>
    /// <param name="trySpeech">vygeneruje ukazku textu s pravidlami a prevodom cisel; <see langword="null" />, ak hlas jazyka nie je nastaveny</param>
    /// <param name="player">prehravac ukazky</param>
    public FPronunciation(IEnumerable<PronunciationRule> rules, bool numbersAsWords, string languageCode, string sampleText,
        Func<string, IReadOnlyList<PronunciationRule>, bool, CancellationToken, Task<GeneratedSound>>? trySpeech, AudioPlayer player)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _try = trySpeech;
        _languageCode = languageCode;
        NumbersAsWords = numbersAsWords;
        _player = player;
        _rules = new BindingList<PronunciationRule>(rules.Select(r => r.Clone()).ToList()) { AllowNew = true };
        lHint.Text = Resources.FPronunciation_Hint;
        dgvRules.AutoGenerateColumns = false;
        dgvRules.DataSource = _rules;
        tbTest.Text = sampleText;
        cboxNumbers.Checked = numbersAsWords;
        // jazyk bez prevodu - volba sa ulozi, ale nic nerobi
        cboxNumbers.Enabled = NumberWords.Supports(languageCode);
        bTry.Enabled = _try is not null;
        lStatus.Text = _try is null ? Resources.FProsody_NoVoice : "";
        ShowResult();
        FormClosed += (_, _) =>
        {
            _cancel.Cancel();
            _cancel.Dispose();
        };
    }

    /// <summary>
    /// Pravidla po OK (bez prazdnych).
    /// </summary>
    public List<PronunciationRule> Rules { get; private set; } = [];

    /// <summary>
    /// Cisla citat slovami po OK.
    /// </summary>
    public bool NumbersAsWords { get; private set; }

    private List<PronunciationRule> CurrentRules() => _rules.Where(r => r.Find.Length > 0).Select(r => r.Clone()).ToList();

    private void ShowResult() =>
        lResult.Text = NumberWords.Apply(Pronunciation.Apply(tbTest.Text, CurrentRules()), cboxNumbers.Checked ? _languageCode : "");

    private void CboxNumbers_CheckedChanged(object? sender, EventArgs e) => ShowResult();

    private void TbTest_TextChanged(object? sender, EventArgs e) => ShowResult();

    private void DgvRules_CellEndEdit(object? sender, DataGridViewCellEventArgs e) => ShowResult();

    private void DgvRules_UserDeletedRow(object? sender, DataGridViewRowEventArgs e) => ShowResult();

    private void DgvRules_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (dgvRules.CurrentCell is not DataGridViewCheckBoxCell)
            return;

        dgvRules.CommitEdit(DataGridViewDataErrorContexts.Commit);
        ShowResult();
    }

    private void MoveRule(int offset)
    {
        if (dgvRules.CurrentRow?.DataBoundItem is not PronunciationRule rule)
            return;

        var index = _rules.IndexOf(rule);
        var target = index + offset;
        if (index < 0 || target < 0 || target >= _rules.Count)
            return;

        dgvRules.EndEdit();
        _rules.RemoveAt(index);
        _rules.Insert(target, rule);
        dgvRules.CurrentCell = dgvRules.Rows[target].Cells[cFind.Index];
        ShowResult();
    }

    private void BUp_Click(object? sender, EventArgs e) => MoveRule(-1);

    private void BDown_Click(object? sender, EventArgs e) => MoveRule(1);

    private async void BTry_Click(object? sender, EventArgs e)
    {
        if (_try is null || string.IsNullOrWhiteSpace(tbTest.Text))
            return;

        dgvRules.EndEdit();
        bTry.Enabled = false;
        lStatus.Text = Resources.FVoiceProfile_Generating;
        try
        {
            var result = await _try(tbTest.Text, CurrentRules(), cboxNumbers.Checked, _cancel.Token);
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
        catch (Exception ex) when (ex is SpeechException or InvalidDataException)
        {
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        if (!IsDisposed)
            bTry.Enabled = true;
    }

    private void BOK_Click(object? sender, EventArgs e)
    {
        dgvRules.EndEdit();
        var rules = CurrentRules();
        foreach (var rule in rules)
        {
            if (Pronunciation.Validate(rule) is not { } error)
                continue;

            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FPronunciation_InvalidRegex, rule.Find, error));
            DialogResult = DialogResult.None;
            return;
        }

        Rules = rules;
        NumbersAsWords = cboxNumbers.Checked;
        DialogResult = DialogResult.OK;
    }
}
