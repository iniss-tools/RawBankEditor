using System.Globalization;
using System.Xml;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

partial class FMain
{
    /// <summary>
    /// Import zvukov z tabulky (Excel, CSV, schranka) do otvoreneho jazyka.
    /// </summary>
    private async void DoImportSounds()
    {
        if (CurrentLanguage is not { } language)
            return;

        SoundImportPlan plan;
        using (var form = new FImportSounds(language))
        {
            if (form.ShowDialog(this) != DialogResult.OK || form.Plan is null)
                return;
            plan = form.Plan;
        }

        var dirErrors = await ApplyImport(language, plan);
        foreach (var error in dirErrors)
            _dialogs.ShowError(error);

        var message = string.Format(CultureInfo.CurrentCulture, Resources.Import_Done, plan.AllSounds.Count(), plan.NewGroups.Count);
        if (plan.Skipped.Count > 0)
            message += "\n\n" + string.Format(CultureInfo.CurrentCulture, Resources.Import_Skipped, plan.Skipped.Count, ListOf(plan.Skipped));
        _dialogs.ShowInfo(message);
    }

    /// <summary>
    /// Prida skontrolovane skupiny a zvuky do jazyka. Nove skupiny dostanu priecinok, zvuky sa prepoja s nahravkami,
    /// ktore v banke uz su, a prevezmu ich dlzku. Cely import sa vracia jednym krokom Spat.
    /// </summary>
    /// <returns>priecinky skupin, ktore sa nepodarilo vytvorit (skupina sa prida aj bez nich).</returns>
    internal async Task<List<string>> ApplyImport(FyzLanguage language, SoundImportPlan plan)
    {
        var dirErrors = new List<string>();
        var groupActions = new List<AddGroupAction>();
        foreach (var group in plan.NewGroups)
        {
            var directory = GroupDirectoryPath(group);
            var created = false;
            try
            {
                WithoutFileWatcher(() => created = _bank.Journal.CreateDirectory(directory, language));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                // zoznam chyb ponukne chybajuci priecinok vytvorit
                Log.Exception(ex);
                dirErrors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Groups_DirFailed, directory, ex.Message));
            }

            var index = language.Groups.Count;
            InsertGroup(group, index);
            groupActions.Add(new AddGroupAction(this, group, index, created ? directory : null));
        }

        foreach (var sound in plan.SoundsForExistingGroups)
        {
            sound.Group.Sounds.Add(sound);
            RelinkSoundFile(sound);
        }

        var soundsAction = plan.SoundsForExistingGroups.Count > 0 ? new AddSoundsAction(this, plan.SoundsForExistingGroups.ToList()) : null;
        RegisterNewAction(new ImportSoundsAction(this, groupActions, soundsAction));
        RefreshSoundViews();

        await FillDurations(plan.AllSounds.ToList());
        RefreshSoundViews();
        CheckProjectState();
        return dirErrors;
    }

    /// <summary>
    /// Dlzka novych zvukov podla ich nahravok (rovnako ako v okne Pridat zvuk); zvuk bez nahravky ma dlzku 0.
    /// </summary>
    private static async Task FillDurations(IEnumerable<FyzSound> sounds)
    {
        foreach (var sound in sounds)
        {
            if (sound.File is not { } file)
                continue;

            if (file.Duration < 0)
                file.Duration = await SoundUtils.GetSoundDuration(file);
            if (file.Duration >= 0)
                sound.Duration = file.Duration;
        }
    }

    /// <summary>
    /// Export otvoreneho jazyka do FyzBank.xml pre INISS2. Predvolene do priecinka jazyka, kde ho INISS2 hlada.
    /// </summary>
    private async void DoExportIniss2()
    {
        if (CurrentLanguage is not { } language)
            return;

        // FYZBANK.DAT predvoleny jazyk nepozna (INISS ho berie z Categori) - INISS2 potrebuje prave jeden
        var isDefault = _dialogs.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.Export_IsDefault, language.Name),
            MessageBoxButtons.YesNoCancel);
        if (isDefault == DialogResult.Cancel)
            return;

        string file;
        using (var dialog = new SaveFileDialog())
        {
            dialog.Filter = Resources.Export_Filter;
            dialog.FileName = FyzBankXmlWriter.FileName;
            dialog.InitialDirectory = language.GetAbsPath(_bank.PathToBank);
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;
            file = dialog.FileName;
        }

        List<string> missing;
        try
        {
            missing = await ExportIniss2(language, file, isDefault == DialogResult.Yes);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or XmlException or ArgumentException)
        {
            Log.Exception(ex);
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Export_Failed, ex.Message));
            return;
        }

        if (missing.Count > 0)
            _dialogs.ShowWarning(string.Format(CultureInfo.CurrentCulture, Resources.Export_DoneMissing, file, missing.Count, ListOf(missing)));
        else
            _dialogs.ShowInfo(string.Format(CultureInfo.CurrentCulture, Resources.Export_Done, file, language.Groups.Sum(g => g.Sounds.Count)));
    }

    /// <summary>
    /// Zapise jazyk do FyzBank.xml. MD5 nahravok sa pocita na pozadi zo snimky zvukov - zoznam sa medzitym moze menit.
    /// </summary>
    /// <param name="language">jazyk s nacitanymi skupinami</param>
    /// <param name="file">cielovy subor</param>
    /// <param name="isDefault">predvoleny jazyk INISS2</param>
    /// <returns>zvuky bez nahravky v tvare skupina/zvuk - INISS2 ich prehra ako ticho.</returns>
    internal async Task<List<string>> ExportIniss2(FyzLanguage language, string file, bool isDefault)
    {
        var sounds = language.Groups.SelectMany(g => g.Sounds).ToList();
        var pathToBank = _bank.PathToBank;
        ChangeStatus(Resources.Export_Status);
        tspbProgress.Visible = true;
        tspbProgress.Style = ProgressBarStyle.Marquee;
        try
        {
            var hashes = await Task.Run(() => FileHashes(sounds, pathToBank));
            FyzBankXmlWriter.Write(file, language, isDefault, DateTimeOffset.Now, s => hashes.GetValueOrDefault(s));
            return sounds.Where(s => hashes.GetValueOrDefault(s) is null).Select(s => s.Group.Key + "/" + s.Key).ToList();
        }
        finally
        {
            tspbProgress.Style = ProgressBarStyle.Blocks;
            ResetStatusAfterTask();
        }
    }

    /// <summary>
    /// MD5 nahravok zvukov; <see langword="null" /> pri zvuku bez nahravky.
    /// </summary>
    private static Dictionary<FyzSound, string?> FileHashes(IEnumerable<FyzSound> sounds, string pathToBank)
    {
        var hashes = new Dictionary<FyzSound, string?>();
        foreach (var sound in sounds)
        {
            var path = sound.GetAbsPath(pathToBank);
            hashes[sound] = File.Exists(path) ? FyzBankXmlWriter.FileMd5(path) : null;
        }

        return hashes;
    }

    /// <summary>
    /// Import zvukov z tabulky - nove skupiny a zvuky existujucich skupin naraz.
    /// </summary>
    public class ImportSoundsAction : Action
    {
        public ImportSoundsAction(FMain form, IReadOnlyList<AddGroupAction> groups, AddSoundsAction? sounds) : base(form)
        {
            Groups = groups;
            Sounds = sounds;
        }

        private IReadOnlyList<AddGroupAction> Groups { get; }
        private AddSoundsAction? Sounds { get; }

        /// <inheritdoc />
        public override string CommandName => Resources.Action_ImportSounds;

        /// <inheritdoc />
        public override void Undo()
        {
            Sounds?.Undo();
            for (var i = Groups.Count - 1; i >= 0; i--)
                Groups[i].Undo();
        }

        /// <inheritdoc />
        public override void Redo()
        {
            foreach (var group in Groups)
                group.Redo();
            Sounds?.Redo();
        }
    }
}
