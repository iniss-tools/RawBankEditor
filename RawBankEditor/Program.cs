using RawBankEditor.Forms;
using RawBankEditor.XML;
using ToolsCore;

namespace RawBankEditor;

internal static class Program
{
    public static FMain MainForm { get; private set; } = null!;

    [STAThread]
    private static void Main()
    {
        GlobData.Session = AppInit.Initialization<RawBankEditorConfig, RawBankEditorStyle>();

        AppInit.Run(GlobData.Config, () => MainForm = new FMain());
    }
}
