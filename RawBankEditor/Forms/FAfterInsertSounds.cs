using ExControls;
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
        var sound = (FyzSound)e.Row!.DataBoundItem!;
        sound.File.Sound = null!;
    }

    private void BOK_Click(object sender, EventArgs e)
    {
        Program.MainForm.RegisterNewAction(new FMain.AddSoundsAction(Program.MainForm, NewSounds));
        foreach (var sound in NewSounds) 
            sound.Group.Sounds.Add(sound);
        Program.MainForm.dgvSounds.ResetBindings();
    }

    private void FAfterInsertSounds_FormClosed(object sender, FormClosedEventArgs e)
    {
        OpenedForm = null;
    }
}