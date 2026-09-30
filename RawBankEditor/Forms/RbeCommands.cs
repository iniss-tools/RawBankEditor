using RawBankEditor.Properties;
using ToolsCore.Commands;

namespace RawBankEditor.Forms;

/// <summary>
/// Prikazy hlavneho okna s predvolenymi klavesovymi skratkami. Identifikator je nazov prvku skratky v config.xml -
/// nesmie sa menit, inak by sa stratili skratky nastavene pouzivatelom.
/// </summary>
internal static class RbeCommands
{
    public static CommandInfo Open => new("Open", Resources.Cmd_Open, Shortcut.CtrlO);
    public static CommandInfo Save => new("Save", Resources.Cmd_Save, Shortcut.CtrlS);
    public static CommandInfo SaveAll => new("SaveAll", Resources.Cmd_SaveAll, Shortcut.CtrlShiftS);
    public static CommandInfo Undo => new("Undo", Resources.Cmd_Undo, Shortcut.CtrlZ);
    public static CommandInfo Redo => new("Redo", Resources.Cmd_Redo, Shortcut.CtrlY);
    public static CommandInfo AddSound => new("AddSound", Resources.Cmd_AddSound, Shortcut.Ins);
    public static CommandInfo DeleteSounds => new("DeleteSounds", Resources.Cmd_DeleteSounds, Shortcut.Del);
    public static CommandInfo MoveSounds => new("MoveSounds", Resources.Cmd_MoveSounds, Shortcut.CtrlM);
    public static CommandInfo RewriteMode => new("RewriteMode", Resources.Cmd_RewriteMode, Shortcut.F1);
    public static CommandInfo WrapTextSoundCol => new("WrapTextSoundCol", Resources.Cmd_WrapTextSoundCol, Shortcut.F3);
    public static CommandInfo GoBack => new("GoBack", Resources.Cmd_GoBack, Shortcut.AltLeftArrow);
    public static CommandInfo GoForward => new("GoForward", Resources.Cmd_GoForward, Shortcut.AltRightArrow);
    public static CommandInfo Search => new("Search", Resources.Cmd_Search, Shortcut.CtrlF);
    public static CommandInfo AddLanguage => new("AddLang", Resources.Cmd_AddLanguage, Shortcut.None);
    public static CommandInfo EditLanguage => new("EditLang", Resources.Cmd_EditLanguage, Shortcut.None);
    public static CommandInfo DeleteLanguage => new("DeleteLang", Resources.Cmd_DeleteLanguage, Shortcut.None);
    public static CommandInfo AppSettings => new("AppSettings", Resources.Cmd_AppSettings, Shortcut.CtrlShiftN);
    public static CommandInfo HighlightProblem => new("HighlightProblem", Resources.Cmd_HighlightProblem, Shortcut.F10);
    public static CommandInfo ResolveProblem => new("ResolveProblem", Resources.Cmd_ResolveProblem, Shortcut.AltF10);

    // prikazy bez nastavitelnej skratky - v prieskumniku a zozname skupin platia pevne klavesy
    public static CommandInfo ConvertSoundsToEwa => Fixed(nameof(ConvertSoundsToEwa));
    public static CommandInfo ConvertSoundsToWav => Fixed(nameof(ConvertSoundsToWav));
    public static CommandInfo ConvertLanguageToEwa => Fixed(nameof(ConvertLanguageToEwa));
    public static CommandInfo ConvertLanguageToWav => Fixed(nameof(ConvertLanguageToWav));
    public static CommandInfo AddGroup => Fixed(nameof(AddGroup));
    public static CommandInfo EditGroup => Fixed(nameof(EditGroup));
    public static CommandInfo DeleteGroup => Fixed(nameof(DeleteGroup));
    public static CommandInfo ConvertGroupToEwa => Fixed(nameof(ConvertGroupToEwa));
    public static CommandInfo ConvertGroupToWav => Fixed(nameof(ConvertGroupToWav));
    public static CommandInfo OpenInExplorer => Fixed(nameof(OpenInExplorer));
    public static CommandInfo PlayFile => Fixed(nameof(PlayFile));
    public static CommandInfo RenameFile => Fixed(nameof(RenameFile));
    public static CommandInfo DeleteFile => Fixed(nameof(DeleteFile));
    public static CommandInfo ConvertFilesToEwa => Fixed(nameof(ConvertFilesToEwa));
    public static CommandInfo ConvertFilesToWav => Fixed(nameof(ConvertFilesToWav));
    public static CommandInfo InfoApp => Fixed(nameof(InfoApp));
    public static CommandInfo Updates => Fixed(nameof(Updates));

    /// <summary>
    /// Prikazy, ktorym sa v nastaveniach da zmenit skratka, v poradi zoznamu v nastaveniach.
    /// </summary>
    public static IReadOnlyList<CommandInfo> All =>
    [
        Open, Save, SaveAll, Undo, Redo, AddSound, DeleteSounds, MoveSounds, RewriteMode, WrapTextSoundCol,
        GoBack, GoForward, Search, AddLanguage, EditLanguage, DeleteLanguage, AppSettings, HighlightProblem, ResolveProblem
    ];

    private static CommandInfo Fixed(string id) => new(id, id, Shortcut.None);
}
