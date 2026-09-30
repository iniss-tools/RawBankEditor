using RawBankEditor.Forms;
using RawBankEditor.Services;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.Tools;

namespace RawBankEditor;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // composition root (skladanie bez kontajnera): nastavenia programu, sluzba banky a dialogy hlavneho okna;
        // okna ich dostavaju explicitne - ziadny staticky pristup k datam
        var bank = new BankEditor(AppInit.Initialization<RawBankEditorConfig, RawBankEditorStyle>());
        var dialogs = new DialogService();

        AppInit.Run(bank.Config, () => new FMain(bank, dialogs));
    }
}
