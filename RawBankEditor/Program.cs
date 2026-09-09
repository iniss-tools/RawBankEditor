using RawBankEditor.Forms;
using ToolsCore;
using ToolsCore.Forms;
using ToolsCore.Tools;
using ToolsCore.XML;

namespace RawBankEditor;

internal static class Program
{
    public static FMain MainForm { get; private set; } = null!;

    /// <summary>
    ///     Hlavní vstupní bod aplikace.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        GlobSettings.LinkUpdater = "http://iniss.6f.sk/gvdeditor-updater/update.txt";
        AppInit.Initialization(out GlobData.Config, out GlobData.Styles, out GlobData.UsingStyle);

        MainForm = new FMain();

        if (GlobData.Config.DebugModeGUI != DebugMode.AppCrash)
        {
            try
            {
                Application.Run(MainForm);
            }
            catch (Exception exception)
            {
                Log.Exception(exception);
                FError.ShowError(GlobData.Config.DebugModeGUI == DebugMode.OnlyMessage ? exception.Message : exception.ToString());
            }
        }
        else
        {
            Application.Run(MainForm);
        }

        Log.Info("Program sa ukončuje\r\n");
    }
}