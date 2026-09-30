using System.Globalization;
using ExControls;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FAfterInsertSounds : Form
{
    // hlavne okno - do jeho historie ide akcia pridania zvukov
    private readonly FMain _main;

    private FAfterInsertSounds(FMain main, FyzSound sound)
    {
        _main = main;
        InitializeComponent();
        this.ApplyThemeAndFonts();

        NewSounds = [sound];
        fyzSoundBindingSource.DataSource = NewSounds;

        // kluc a nazov sa kontroluju priebezne - rovnake pravidla ako v okne Pridat zvuk
        NewSounds.ListChanged += (_, _) => BeginInvoke(ShowProblems);
        dgvFilesSounds.CellEndEdit += (_, _) => ShowProblems();
        Shown += (_, _) => ShowProblems();
    }

    /// <summary>
    /// Oznaci riadky, ktorych kluc alebo nazov koliduje so zvukom skupiny alebo s inym novym zvukom.
    /// </summary>
    /// <returns>Problemy podla zvuku.</returns>
    private Dictionary<FyzSound, string> ShowProblems()
    {
        var problems = SoundRules.ValidateNew(NewSounds);
        foreach (DataGridViewRow row in dgvFilesSounds.Rows)
        {
            var sound = row.DataBoundItem as FyzSound;
            row.Cells[cSoundKey.Index].ErrorText = sound is not null && problems.TryGetValue(sound, out var text) ? text : "";
        }

        return problems;
    }

    private static FAfterInsertSounds? OpenedForm { get; set; }

    private ExBindingList<FyzSound> NewSounds { get; }

    internal static void CreateOrUseExistingForm(FMain main, FyzSound sound)
    {
        if (OpenedForm is null)
        {
            OpenedForm = new FAfterInsertSounds(main, sound);
            OpenedForm.ShowDialog(main);
        }
        else
        {
            OpenedForm.NewSounds.Add(sound);
        }
    }

    private void DgvFilesSounds_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (cFilePath.Index != e.ColumnIndex)
            return;
        
        e.Value = NewSounds[e.RowIndex].GetAbsPath("");
        e.FormattingApplied = true;
    }

    private void DgvFilesSounds_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
    {
        // odstraneny riadok = subor ostane v banke bez udajov o zvuku
        var sound = (FyzSound)e.Row!.DataBoundItem!;
        if (sound.File?.Sound == sound)
            sound.File.Sound = null!;
    }

    private void BOK_Click(object sender, EventArgs e)
    {
        dgvFilesSounds.EndEdit();
        var problems = ShowProblems();
        if (problems.Count > 0)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FAfterInsertSounds_Problems,
                string.Join("\n", problems.Values.Distinct().Take(10))));
            return;
        }

        // tlacidlo nema DialogResult v navrhu - okno zavrie az toto
        DialogResult = DialogResult.OK;
        if (NewSounds.Count == 0)
            return;

        _main.RegisterNewAction(new FMain.AddSoundsAction(_main, NewSounds.ToList()));
        foreach (var sound in NewSounds) 
            sound.Group.Sounds.Add(sound);
        _main.RefreshSoundViews();
    }

    private void FAfterInsertSounds_FormClosed(object sender, FormClosedEventArgs e)
    {
        OpenedForm = null;

        // Zrusit (aj kriz) - subory ostanu v banke bez udajov o zvuku
        if (DialogResult == DialogResult.OK)
            return;

        foreach (var sound in NewSounds)
            if (sound.File?.Sound == sound)
                sound.File.Sound = null!;
    }
}