using System.Globalization;
using RawBankEditor.Entities;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using Serilog;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Forms;

/// <summary>
/// Vlastnosti banky INISS2: predvoleny jazyk, nazvy podla klucov a kontrola nahravok.
/// </summary>
partial class FMain
{
    // nahravky, ktore pri poslednej kontrole jazyka nezodpovedali udajom banky
    private readonly Dictionary<FyzLanguage, List<RecordingMismatch>> _recordingMismatches = [];
    private bool _checkingRecordings;

    private bool IsIniss2 => _bank.Project?.Store.IsIniss2 == true;

    /// <summary>
    /// Prisposobi okno otvorenej banke - INISS2 nazvy zvukov neuklada, nazov je kluc.
    /// </summary>
    private void ApplyBankKind()
    {
        var isIniss2 = IsIniss2;
        cSoundName.ReadOnly = isIniss2;
        tsmimDefaultLanguage.Visible = isIniss2;
        tsmiDefaultLanguage.Visible = isIniss2;
        tsmimCheckRecordings.Visible = isIniss2;
        tsmiCheckRecordings.Visible = isIniss2;
        _recordingMismatches.Clear();
    }

    private void DoSetDefaultLanguage()
    {
        var language = CurrentLanguage!;
        var action = new DefaultLanguageAction(this, _bank.Project!.Languages.FirstOrDefault(l => l.IsBasic), language);
        ChangeDefaultLanguage(language);
        RegisterNewAction(action);
    }

    /// <summary>
    /// Nastavi predvoleny jazyk banky - vzdy najviac jeden.
    /// </summary>
    /// <param name="language">novy predvoleny jazyk; <see langword="null" /> = ziadny</param>
    internal void ChangeDefaultLanguage(FyzLanguage? language)
    {
        foreach (var lang in _bank.Project!.Languages)
            lang.IsBasic = ReferenceEquals(lang, language);

        UpdateCommandStates();
        CheckProjectState();
    }

    /// <summary>
    /// Porovna nahravky otvoreneho jazyka s MD5 a dlzkou v banke. Citaju sa vsetky nahravky jazyka, preto na pozadi.
    /// </summary>
    private async void DoCheckRecordings()
    {
        var project = _bank.Project!;
        var language = CurrentLanguage!;
        var sounds = language.Groups.SelectMany(g => g.Sounds).ToList();

        Log.Information("Kontrola nahravok jazyka {Language} ({Count} zvukov)", language.Key, sounds.Count);
        _checkingRecordings = true;
        UpdateCommandStates();
        ChangeStatus(Resources.CheckRecordings_Status);
        tspbProgress.Visible = true;
        tspbProgress.Style = ProgressBarStyle.Marquee;

        List<RecordingMismatch> mismatches;
        try
        {
            mismatches = await RecordingCheck.FindAsync(sounds);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Log.Error(ex, "Kontrola nahravok jazyka {Language} zlyhala", language.Key);
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.CheckRecordings_Failed, ex.Message));
            return;
        }
        finally
        {
            _checkingRecordings = false;
            tspbProgress.Style = ProgressBarStyle.Blocks;
            ResetStatusAfterTask();
            UpdateCommandStates();
        }

        // banka sa medzitym zatvorila alebo sa otvorila ina
        if (!ReferenceEquals(_bank.Project, project))
            return;

        Log.Information("Kontrola nahravok jazyka {Language}: {Count} nezodpoveda udajom banky", language.Key, mismatches.Count);
        _recordingMismatches[language] = mismatches;
        RefreshMessages();

        if (mismatches.Count == 0)
        {
            _dialogs.ShowInfo(string.Format(CultureInfo.CurrentCulture, Resources.CheckRecordings_Ok, sounds.Count));
            return;
        }

        var answer = _dialogs.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.CheckRecordings_Found, mismatches.Count, sounds.Count),
            MessageBoxButtons.YesNo);
        if (answer == DialogResult.Yes)
            ApplyRecordingData(mismatches);
    }

    /// <summary>
    /// Prepise MD5 a dlzku zvukov podla nahravok na disku (jeden krok Spat).
    /// </summary>
    internal void ApplyRecordingData(IReadOnlyList<RecordingMismatch> mismatches)
    {
        var action = new RecordingDataAction(this, mismatches);
        action.Redo();
        RegisterNewAction(action);
    }

    /// <summary>
    /// Vyriesene nahravky prestanu byt v zozname chyb - alebo sa do neho vratia pri kroku Spat.
    /// </summary>
    internal void UpdateRecordingMismatches(IReadOnlyList<RecordingMismatch> changed, bool resolved)
    {
        if (changed.Count > 0)
        {
            var language = changed[0].Sound.Language;
            var mismatches = _recordingMismatches.GetValueOrDefault(language) ?? [];
            _recordingMismatches[language] = resolved ? mismatches.Except(changed).ToList() : mismatches.Union(changed).ToList();
        }

        RefreshSoundViews();
        RefreshMessages();
        CheckProjectState();
    }

    /// <summary>
    /// Doplni do zoznamu chyb jazyka nahravky z poslednej kontroly. Zvuky znovu nacitaneho jazyka su ine objekty a vypadnu.
    /// </summary>
    private void AddRecordingMessages(FyzLanguage language)
    {
        if (!_recordingMismatches.TryGetValue(language, out var mismatches))
            return;

        var sounds = language.Groups.SelectMany(g => g.Sounds).ToHashSet();
        _bank.Project!.Messages[language].AddRange(mismatches.Where(m => sounds.Contains(m.Sound)).Select(m => new RecordingChanged(m)));
    }

    /// <summary>
    /// Akcia pri zmene predvoleneho jazyka.
    /// </summary>
    public class DefaultLanguageAction(FMain form, FyzLanguage? previous, FyzLanguage language) : Action(form)
    {
        /// <inheritdoc />
        public override string CommandName => Resources.Action_DefaultLanguage;

        /// <inheritdoc />
        public override void Undo() => Form.ChangeDefaultLanguage(previous);

        /// <inheritdoc />
        public override void Redo() => Form.ChangeDefaultLanguage(language);
    }

    /// <summary>
    /// Akcia pri aktualizacii MD5 a dlzky zvukov podla nahravok.
    /// </summary>
    public class RecordingDataAction(FMain form, IReadOnlyList<RecordingMismatch> mismatches) : Action(form)
    {
        private readonly List<(FyzSound Sound, string? Md5, int Duration)> _previous = mismatches.Select(m => (m.Sound, m.Sound.Md5, m.Sound.Duration)).ToList();

        /// <inheritdoc />
        public override string CommandName => Resources.Action_RecordingData;

        /// <inheritdoc />
        public override void Undo()
        {
            foreach (var (sound, md5, duration) in _previous)
            {
                sound.Md5 = md5;
                sound.Duration = duration;
            }

            Form.UpdateRecordingMismatches(mismatches, false);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            foreach (var mismatch in mismatches)
            {
                mismatch.Sound.Md5 = mismatch.Md5;
                mismatch.Sound.Duration = mismatch.Duration;
            }

            Form.UpdateRecordingMismatches(mismatches, true);
        }
    }
}
