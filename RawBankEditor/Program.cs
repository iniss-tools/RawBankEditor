using RawBankEditor.Forms;
using ToolsCore;

namespace RawBankEditor;

internal static class Program
{
    public static FMain MainForm { get; private set; } = null!;

    [STAThread]
    private static void Main()
    {
        AppInit.Initialization(out GlobData.Config, out GlobData.Styles, out GlobData.UsingStyle);

        AppInit.Run(GlobData.Config, () => MainForm = new FMain());
    }
}
