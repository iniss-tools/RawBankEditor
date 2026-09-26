using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

partial class FMain
{
    private void DoConvertSoundsToEwa(object sender, EventArgs e) => ConvertSelectedSounds(true);

    private void DoConvertSoundsToWav(object sender, EventArgs e) => ConvertSelectedSounds(false);

    private void DoConvertGroupToEwa(object sender, EventArgs e) => ConvertSelectedGroup(true);

    private void DoConvertGroupToWav(object sender, EventArgs e) => ConvertSelectedGroup(false);

    private void DoConvertLangToEwa(object sender, EventArgs e) => ConvertCurrentLanguage(true);

    private void DoConvertLangToWav(object sender, EventArgs e) => ConvertCurrentLanguage(false);

    private void DoConvertFilesToEwa(object sender, EventArgs e) => ConvertSelectedFiles(true);

    private void DoConvertFilesToWav(object sender, EventArgs e) => ConvertSelectedFiles(false);

    private void ConvertSelectedSounds(bool toEwa)
    {
        if (!ConfirmConversion(Resources.FMain_DoConvertSounds, toEwa))
            return;

        var sounds = dgvSounds.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<FyzSound>();
        StartConversion(FilesOf(sounds), toEwa, "Konvertovanie vybraných zvukov", "Konvertujem vybrané zvuky");
    }

    private void ConvertSelectedGroup(bool toEwa)
    {
        if (dgvGroups.IsSelectionEmpty() || !ConfirmConversion(Resources.FMain_DoConvertGroup, toEwa))
            return;

        var group = (FyzGroup)dgvGroups.SelectedRows[0].DataBoundItem!;
        StartConversion(FilesOf(group.Sounds), toEwa, "Konvertovanie zvukov skupiny", "Konvertujem skupinu zvukov");
    }

    private void ConvertCurrentLanguage(bool toEwa)
    {
        if (CurrentLanguage?.Groups is null || !ConfirmConversion(Resources.FMain_DoConvertLang, toEwa))
            return;

        StartConversion(FilesOf(CurrentLanguage.Groups.SelectMany(g => g.Sounds)), toEwa, "Konvertovanie zvukov jazyka", "Konvertujem zvuky jazyka");
    }

    private void ConvertSelectedFiles(bool toEwa)
    {
        if (dgvExplorer.IsSelectionEmpty() || !ConfirmConversion(Resources.FMain_DoConvertFiles, toEwa))
            return;

        var elements = dgvExplorer.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<FileSystemElement>()
            .Where(el => el is not BackButtonElement);
        StartConversion(SoundUtils.SoundFilesIn(elements), toEwa, "Konvertovanie vybraných súborov", "Konvertujem vybrané súbory");
    }

    private static bool ConfirmConversion(string question, bool toEwa)
        => Utils.ShowQuestion(string.Format(question, toEwa ? SoundUtils.EWA_EXT : SoundUtils.WAV_EXT)) == DialogResult.Yes;

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
    ///     Skonvertuje subory na pozadi. Sledovanie suborov je pocas konverzie vypnute - prvky prieskumnika aj nazvy
    ///     suborov zvukov upravi sama konverzia.
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
            result = await Task.Run(() => SoundUtils.ConvertSoundFiles(files, toEwa,
                () => BeginInvoke(() => tspbProgress.Increment(1))));
        }
        catch (Exception ex)
        {
            Log.Exception(ex);
            Utils.ShowError(ex.Message);
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

    private static void ReportConversion(SoundUtils.ConvertResult result, bool toEwa)
    {
        var ext = toEwa ? SoundUtils.EWA_EXT : SoundUtils.WAV_EXT;
        if (result.Skipped.Count > 0)
            Utils.ShowWarning($"Niektoré súbory sa neskonvertovali, lebo vedľa nich už je súbor s príponou {ext}:\n\n{List(result.Skipped)}");
        if (result.Failed.Count > 0)
            Utils.ShowError($"Niektoré súbory sa nepodarilo skonvertovať:\n\n{List(result.Failed)}");

        static string List(List<string> items)
            => string.Join("\n", items.Take(10)) + (items.Count > 10 ? $"\n… a ďalšie ({items.Count - 10})" : "");
    }
}
