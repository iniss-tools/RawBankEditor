using RawBankEditor.Entities;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.XML;

namespace RawBankEditor;

internal static class GlobData
{
    /// <summary>
    /// Nastavenia programu (konfiguracia a styly), vytvorene pri starte.
    /// </summary>
    public static AppSession<RawBankEditorConfig, RawBankEditorStyle> Session { get; set; } = null!;

    /// <summary>
    /// Otvorena banka zvukov; <see langword="null" />, kym pouzivatel ziadnu neotvori.
    /// </summary>
    public static RawBankProject? OpenedProject { get; set; }

    public static RawBankEditorConfig Config { get => Session.Config; set => Session.Config = value; }
    public static Styles<RawBankEditorStyle> Styles { get => Session.Styles; set => Session.Styles = value; }
    public static RawBankEditorStyle UsingStyle { get => Session.UsingStyle; set => Session.UsingStyle = value; }
}