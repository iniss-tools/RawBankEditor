using ExControls;
using RawBankEditor.Tools;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FAfterInsertSounds : Form
{
    private FAfterInsertSounds(FyzSound sound)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        NewSounds = new ExBindingList<FyzSound> { sound };
        fyzSoundBindingSource.DataSource = NewSounds;

        // kluc a nazov sa kontroluju priebezne - rovnake pravidla ako v okne Pridat zvuk
        NewSounds.ListChanged += (_, _) => BeginInvoke(ShowProblems);
        dgvFilesSounds.CellEndEdit += (_, _) => ShowProblems();
        Shown += (_, _) => ShowProblems();
    }

    /// <summary>
    ///     Oznaci riadky, ktorych kluc alebo nazov koliduje so zvukom skupiny alebo s inym novym zvukom.
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

    public static void CreateOrUseExistingForm(FyzSound sound)
    {
        if (OpenedForm is null)
        {
            OpenedForm = new FAfterInsertSounds(sound);
            OpenedForm.ShowDialog(Program.MainForm);
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
            Utils.ShowError("Niektoré zvuky sa nedajú pridať – opravte kľúč alebo názov, alebo riadok odstráňte (Del):\n\n"
                            + string.Join("\n", problems.Values.Distinct().Take(10)));
            return;
        }

        // tlacidlo nema DialogResult v navrhu - okno zavrie az toto
        DialogResult = DialogResult.OK;
        if (NewSounds.Count == 0)
            return;

        Program.MainForm.RegisterNewAction(new FMain.AddSoundsAction(Program.MainForm, NewSounds.ToList()));
        foreach (var sound in NewSounds) 
            sound.Group.Sounds.Add(sound);
        Program.MainForm.RefreshSoundViews();
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