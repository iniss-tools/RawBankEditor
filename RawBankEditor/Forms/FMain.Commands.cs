using ToolsCore.Commands;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FMain
{
    private readonly CommandSet _commands = [];

    private bool ProjectOpen => _bank.Project is not null;

    // otvoreny jazyk sa nacital bez chyby - az potom sa daju upravovat skupiny a zvuky
    private bool LanguageReady => ProjectOpen && _languageLoaded;

    // ulozit sa da nacitany jazyk alebo banka bez jazyka (FYZBANK.DAT, napr. po odstraneni posledneho jazyka)
    private bool CanSave => ProjectOpen && !_readingLanguage && (_languageLoaded || CurrentLanguage is null);

    private List<object?> ExplorerSelection => dgvExplorer.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).ToList();

    /// <summary>
    /// Vytvori prikazy hlavneho okna a naviaze ich na tlacidla, polozky ponuky a kontextovych ponuk.
    /// </summary>
    private void CreateCommands()
    {
        void Add(CommandInfo info, System.Action execute, Func<bool>? canExecute, params ToolStripItem[] items) =>
            _commands.Add(info, execute, canExecute).Bind(items);

        Func<bool> ready = () => LanguageReady;
        Func<bool> soundsSelected = () => LanguageReady && !dgvSounds.IsSelectionEmpty();

        Add(RbeCommands.Open, DoOpenDir, null, tsmimOpen, tsbOpen);
        Add(RbeCommands.Save, () => SaveBank(false), () => CanSave, tsmimSave, tsbSave);
        Add(RbeCommands.SaveAll, () => SaveBank(true), () => CanSave, tsmimSaveAll, tsbSaveAll);
        Add(RbeCommands.Undo, DoUndo, () => changeManager.CanUndo, tsmimUndo, tsbUndo);
        Add(RbeCommands.Redo, DoRedo, () => changeManager.CanRedo, tsmimRedo, tsbRedo);

        Add(RbeCommands.AddSound, DoAddSound, ready, tsmimAddSound, tsbAddSound, cmiAddSound);
        Add(RbeCommands.DeleteSounds, DoDeleteSounds, soundsSelected, tsmimDeleteSound, tsbDeleteSound, cmiDeleteSound);
        Add(RbeCommands.MoveSounds, DoMoveSounds, soundsSelected, tsmimMoveSounds, tsbMoveSounds, cmiMoveSounds);
        Add(RbeCommands.ConvertSoundsToEwa, () => ConvertSelectedSounds(true), ready, tsmimConvertSoundsToEwa, tsbConvertSoundsToEwa,
            cmiConvertSoundsToEwa);
        Add(RbeCommands.ConvertSoundsToWav, () => ConvertSelectedSounds(false), ready, tsmimConvertSoundsToWav, tsbConvertSoundsToWav,
            cmiConvertSoundsToWav);

        // prepinace sa prepinaju samy (CheckOnClick) - prikaz ich prepne len pri klavesovej skratke
        _commands.Add(RbeCommands.RewriteMode, () => tsmimRewriteMode.Checked = !tsmimRewriteMode.Checked, ready)
            .BindState(tsmimRewriteMode, tsbRewriteMode);
        _commands.Add(RbeCommands.WrapTextSoundCol, () => tsmimWrapTextSoundCol.Checked = !tsmimWrapTextSoundCol.Checked)
            .BindState(tsmimWrapTextSoundCol, tsbWrapTextSoundCol);

        Add(RbeCommands.GoBack, DoGoBack, () => moveManager.CanBackward, tsmimGoBack, tsbGoBack);
        Add(RbeCommands.GoForward, DoGoForward, () => moveManager.CanForward, tsmimGoForward, tsbGoForward);
        Add(RbeCommands.Search, DoSearch, ready, tsmimSearch, tsbSearch);

        Add(RbeCommands.AddLanguage, DoAddLanguage, () => ProjectOpen, tsmimAddLanguage, tsmiAddLanguage);
        Add(RbeCommands.EditLanguage, DoEditLanguage, () => ProjectOpen && CurrentLanguage != null, tsmimEditLanguage, tsmiEditLanguage);
        Add(RbeCommands.DeleteLanguage, DoDeleteLanguage, () => ProjectOpen && CurrentLanguage != null, tsmimDeleteLanguage,
            tsmiDeleteLanguage);
        // konverzia prechadza skupiny jazyka - len nacitany jazyk
        Add(RbeCommands.ConvertLanguageToEwa, () => ConvertCurrentLanguage(true), () => LanguageReady && CurrentLanguage != null,
            tsmimConvertLangToEwa, tsmiConvertLangToEwa);
        Add(RbeCommands.ConvertLanguageToWav, () => ConvertCurrentLanguage(false), () => LanguageReady && CurrentLanguage != null,
            tsmimConvertLangToWav, tsmiConvertLangToWav);
        Add(RbeCommands.ImportSounds, DoImportSounds, () => LanguageReady && CurrentLanguage != null, tsmimImportSounds, tsmiImportSounds);
        Add(RbeCommands.ExportIniss2, DoExportIniss2, () => LanguageReady && CurrentLanguage != null, tsmimExportIniss2, tsmiExportIniss2);

        Add(RbeCommands.AddGroup, DoAddGroup, ready, tsbAddGroup, cmiAddGroup);
        Add(RbeCommands.EditGroup, DoEditGroup, ready, tsbEditGroup, cmiEditGroup);
        Add(RbeCommands.DeleteGroup, DoDeleteGroup, ready, tsbDeleteGroup, cmiDeleteGroup);
        Add(RbeCommands.ConvertGroupToEwa, () => ConvertSelectedGroup(true), () => LanguageReady && !dgvGroups.IsSelectionEmpty(),
            tsbConvertGroupToEwa, cmiConvertGroupToEwa);
        Add(RbeCommands.ConvertGroupToWav, () => ConvertSelectedGroup(false), () => LanguageReady && !dgvGroups.IsSelectionEmpty(),
            tsbConvertGroupToWav, cmiConvertGroupToWav);

        // prieskumnik: tlacidlo Spat (o priecinok vyssie) sa neda premenovat ani odstranit
        Add(RbeCommands.OpenInExplorer, DoOpenFileInExplorer, () => ExplorerSelection.Count > 0, tsbOpenInExplorer, cmiOpenInExplorer);
        Add(RbeCommands.PlayFile, DoPlayFile, () => ExplorerSelection is [SoundFileElement], tsbPlay, cmiPlay);
        Add(RbeCommands.RenameFile, DoRenameFile, () => ExplorerSelection is [not BackButtonElement], tsbRenameFileDir, cmiRenameFileDir);
        Add(RbeCommands.DeleteFile, DoDeleteFile, () => ExplorerSelection is { Count: > 0 } items && !items.Any(i => i is BackButtonElement),
            tsbDeleteFileDir, cmiDeleteFileDir);
        Add(RbeCommands.ConvertFilesToEwa, () => ConvertSelectedFiles(true), () => ExplorerSelection is { Count: > 1 } or [SoundFileElement],
            tsbConvertFilesToEwa, cmiConvertToEwaFile);
        Add(RbeCommands.ConvertFilesToWav, () => ConvertSelectedFiles(false), () => ExplorerSelection is { Count: > 1 } or [SoundFileElement],
            tsbConvertFilesToWav, cmiConvertToWavFile);

        Add(RbeCommands.HighlightProblem, DoFindProblem, ready, tsbHighlightProblem, cmiHighlightProblem);
        Add(RbeCommands.ResolveProblem, DoSolveProblem, ready, tsbResolveProblem, cmiResolveProblem);

        Add(RbeCommands.AppSettings, ShowAppSettings, null, tsmimAppSettings, tsbAppSettings);
        Add(RbeCommands.InfoApp, ShowInfoApp, null, tsmimInfoApp, tsbInfoApp);
        Add(RbeCommands.Updates, () => Utils.OpenShell(LinkConsts.Update), null, tsmimUpdates);
    }

    /// <summary>
    /// Povoli alebo zakaze prikazy podla stavu (otvorena banka, nacitany jazyk, vyber, historia zmien).
    /// </summary>
    private void UpdateCommandStates()
    {
        _commands.UpdateStates();

        // rozbalovacie ponuky jazykov bez vlastneho prikazu
        tsbLangsSettings.Enabled = ProjectOpen;
        tsmimLangsSettings.Enabled = ProjectOpen;
    }
}
