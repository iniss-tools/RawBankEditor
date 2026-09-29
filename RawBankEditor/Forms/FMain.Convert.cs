using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

partial class FMain
{
    private void ConvertSelectedSounds(bool toEwa)
    {
        if (!ConfirmConversion(Resources.FMain_DoConvertSounds, toEwa))
            return;

        var sounds = dgvSounds.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<FyzSound>();
        StartConversion(FilesOf(sounds), toEwa, Resources.Convert_SoundsTitle, Resources.Convert_SoundsStatus);
    }

    private void ConvertSelectedGroup(bool toEwa)
    {
        if (dgvGroups.IsSelectionEmpty() || !ConfirmConversion(Resources.FMain_DoConvertGroup, toEwa))
            return;

        var group = (FyzGroup)dgvGroups.SelectedRows[0].DataBoundItem!;
        StartConversion(FilesOf(group.Sounds), toEwa, Resources.Convert_GroupTitle, Resources.Convert_GroupStatus);
    }

    private void ConvertCurrentLanguage(bool toEwa)
    {
        if (CurrentLanguage?.Groups is null || !ConfirmConversion(Resources.FMain_DoConvertLang, toEwa))
            return;

        StartConversion(FilesOf(CurrentLanguage.Groups.SelectMany(g => g.Sounds)), toEwa, Resources.Convert_LanguageTitle,
            Resources.Convert_LanguageStatus);
    }

    private void ConvertSelectedFiles(bool toEwa)
    {
        if (dgvExplorer.IsSelectionEmpty() || !ConfirmConversion(Resources.FMain_DoConvertFiles, toEwa))
            return;

        var elements = dgvExplorer.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<FileSystemElement>()
            .Where(el => el is not BackButtonElement);
        StartConversion(SoundUtils.SoundFilesIn(elements), toEwa, Resources.Convert_FilesTitle, Resources.Convert_FilesStatus);
    }

    private bool ConfirmConversion(string question, bool toEwa)
        => _dialogs.ShowQuestion(string.Format(CultureInfo.CurrentCulture, question, toEwa ? SoundUtils.EWA_EXT : SoundUtils.WAV_EXT)) == DialogResult.Yes;

    // nahravky zvukov, ktore subor maju
    private static List<SoundFileElement> FilesOf(IEnumerable<FyzSound> sounds)
        => sounds.Select(s => s.File).OfType<SoundFileElement>().ToList();

    private async void StartConversion(List<SoundFileElement> files, bool toEwa, string commandName, string status)
    {
        var result = await ConvertInBackground(files, toEwa, status);
        // Spat skonvertuje naspat len subory, ktore sa naozaj skonvertovali
        if (result.Converted.Count > 0)
            RegisterNewAction(new ConvertFilesAction(this, result.Converted, toEwa, commandName));
    }

    /// <summary>
    /// Skonvertuje subory na pozadi. Sledovanie suborov je pocas konverzie vypnute - prvky prieskumnika aj nazvy
    /// suborov zvukov upravi sama konverzia. Povodne subory sa odstrania cez zurnal banky (zahodenie zmien ich vrati).
    /// </summary>
    internal async Task<SoundUtils.ConvertResult> ConvertInBackground(IReadOnlyCollection<SoundFileElement> files, bool toEwa, string status)
    {
        ChangeStatus(status);
        tspbProgress.Visible = true;
        tspbProgress.Style = ProgressBarStyle.Blocks;
        tspbProgress.Minimum = 0;
        tspbProgress.Maximum = Math.Max(files.Count, 1);
        tspbProgress.Value = 0;

        var result = new SoundUtils.ConvertResult();
        var watching = fileSystemWatcher.EnableRaisingEvents;
        RawBankExplorer.ConvertSoundIsHandled = true;
        fileSystemWatcher.EnableRaisingEvents = false;
        try
        {
            var journal = _bank.Journal;
            var scope = CurrentLanguage;
            result = await Task.Run(() => SoundUtils.ConvertSoundFiles(files, toEwa,
                () => BeginInvoke(() => tspbProgress.Increment(1)), journal, scope));
        }
        catch (Exception ex)
        {
            Log.Exception(ex);
            _dialogs.ShowError(ex.Message);
        }
        finally
        {
            fileSystemWatcher.EnableRaisingEvents = watching;
            // udalosti, ktore watcher zaradil do fronty okna este pred vypnutim, sa spracuju az po tomto
            BeginInvoke(() => RawBankExplorer.ConvertSoundIsHandled = false);
        }

        ResetStatusAfterTask();
        MenuSounds?.ResetBindings();
        ReportConversion(result, toEwa);
        return result;
    }

    private void ReportConversion(SoundUtils.ConvertResult result, bool toEwa)
    {
        var ext = toEwa ? SoundUtils.EWA_EXT : SoundUtils.WAV_EXT;
        if (result.Skipped.Count > 0)
            _dialogs.ShowWarning(string.Format(CultureInfo.CurrentCulture, Resources.Convert_Skipped, ext, ListOf(result.Skipped)));
        if (result.Failed.Count > 0)
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Convert_Failed, ListOf(result.Failed)));
    }
}
