using System.Reflection;
using System.Xml.Serialization;
using ToolsCore.XML;

namespace RawBankEditor.XML;

/// <summary>
///     Obsahuje zoznam všetkých možných klávesových skratiek pre program.
/// </summary>
public record AppShortcuts()
{
    private static readonly Type ClassType = typeof(AppShortcuts);

    [XmlIgnore]
    private static readonly Dictionary<string, (Shortcut shortcut, string name)> Props = new()
    {
        [nameof(Open)] = (Shortcut.CtrlO, "Otvoriť"),
        [nameof(Save)] = (Shortcut.CtrlS, "Uložiť"),
        [nameof(SaveAll)] = (Shortcut.CtrlShiftS, "Uložiť všetko"),
        [nameof(Undo)] = (Shortcut.CtrlZ, "Späť"),
        [nameof(Redo)] = (Shortcut.CtrlY, "Znovu"),
        [nameof(AddSound)] = (Shortcut.Ins, "Pridať zvuk"),
        [nameof(DeleteSounds)] = (Shortcut.Del, "Odstrániť zvuky"),
        [nameof(MoveSounds)] = (Shortcut.CtrlM, "Presunúť zvuky"),
        [nameof(RewriteMode)] = (Shortcut.F1, "Prepisovací mód"),
        [nameof(WrapTextSoundCol)] = (Shortcut.F3, "Prispôsobiť text hlásenia v bunkách"),
        [nameof(GoBack)] = (Shortcut.AltLeftArrow, "Prejsť späť"),
        [nameof(GoForward)] = (Shortcut.AltRightArrow, "Prejsť dopredu"),
        [nameof(Search)] = (Shortcut.CtrlF, "Hľadať"),
        [nameof(AddLanguage)] = (Shortcut.None, "Pridať jazyk"),
        [nameof(EditLanguage)] = (Shortcut.None, "Upraviť jazyk"),
        [nameof(DeleteLanguage)] = (Shortcut.None, "Odstrániť jazyk"),
        [nameof(AppSettings)] = (Shortcut.CtrlShiftN, "Nastavenia programu"),
        [nameof(HighlightProblem)] = (Shortcut.F10, "Zvýrazniť problem"),
        [nameof(ResolveProblem)] = (Shortcut.AltF10, "Vyriešiť problém")
    };

    #region Fields

    private CmdShortcut _open = InitShortcut(nameof(Open));
    private CmdShortcut _save = InitShortcut(nameof(Save));
    private CmdShortcut _saveAll = InitShortcut(nameof(SaveAll));
    private CmdShortcut _undo = InitShortcut(nameof(Undo));
    private CmdShortcut _redo = InitShortcut(nameof(Redo));
    private CmdShortcut _addSound = InitShortcut(nameof(AddSound));
    private CmdShortcut _deleteSounds = InitShortcut(nameof(DeleteSounds));
    private CmdShortcut _moveSounds = InitShortcut(nameof(MoveSounds));
    private CmdShortcut _rewriteMode = InitShortcut(nameof(RewriteMode));
    private CmdShortcut _wrapTextSoundCol = InitShortcut(nameof(WrapTextSoundCol));
    private CmdShortcut _goBack = InitShortcut(nameof(GoBack));
    private CmdShortcut _goForward = InitShortcut(nameof(GoForward));
    private CmdShortcut _search = InitShortcut(nameof(Search));
    private CmdShortcut _addLang = InitShortcut(nameof(AddLanguage));
    private CmdShortcut _editLang = InitShortcut(nameof(EditLanguage));
    private CmdShortcut _deleteLang = InitShortcut(nameof(DeleteLanguage));
    private CmdShortcut _appSettings = InitShortcut(nameof(AppSettings));
    private CmdShortcut _highlightProblem = InitShortcut(nameof(HighlightProblem));
    private CmdShortcut _resolveProblem = InitShortcut(nameof(ResolveProblem));

    #endregion

    #region Properties

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Open")]
    public CmdShortcut Open
    {
        get => _open ??= InitShortcut(nameof(Open));
        set
        {
            _open = value;
            AssignShortcutProps(ref _open, nameof(Open));
        }
    }

    /// <summary>
    ///     Skratka pre uloženie jazyka banky.
    /// </summary>
    [XmlElement("Save")]
    public CmdShortcut Save
    {
        get => _save ??= InitShortcut(nameof(Save));
        set
        {
            _save = value;
            AssignShortcutProps(ref _save, nameof(Save));
        }
    }

    /// <summary>
    ///     Skratka pre uloženie vsetkych jazykov banky.
    /// </summary>
    [XmlElement("SaveAll")]
    public CmdShortcut SaveAll
    {
        get => _saveAll ??= InitShortcut(nameof(SaveAll));
        set
        {
            _saveAll = value;
            AssignShortcutProps(ref _saveAll, nameof(SaveAll));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Undo")]
    public CmdShortcut Undo
    {
        get => _undo ??= InitShortcut(nameof(Undo));
        set
        {
            _undo = value;
            AssignShortcutProps(ref _undo, nameof(Undo));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Redo")]
    public CmdShortcut Redo
    {
        get => _redo ??= InitShortcut(nameof(Redo));
        set
        {
            _redo = value;
            AssignShortcutProps(ref _redo, nameof(Redo));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("AddSound")]
    public CmdShortcut AddSound
    {
        get => _addSound ??= InitShortcut(nameof(AddSound));
        set
        {
            _addSound = value;
            AssignShortcutProps(ref _addSound, nameof(AddSound));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("DeleteSounds")]
    public CmdShortcut DeleteSounds
    {
        get => _deleteSounds ??= InitShortcut(nameof(DeleteSounds));
        set
        {
            _deleteSounds = value;
            AssignShortcutProps(ref _deleteSounds, nameof(DeleteSounds));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("MoveSounds")]
    public CmdShortcut MoveSounds
    {
        get => _moveSounds ??= InitShortcut(nameof(MoveSounds));
        set
        {
            _moveSounds = value;
            AssignShortcutProps(ref _moveSounds, nameof(MoveSounds));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("RewriteMode")]
    public CmdShortcut RewriteMode
    {
        get => _rewriteMode ??= InitShortcut(nameof(RewriteMode));
        set
        {
            _rewriteMode = value;
            AssignShortcutProps(ref _rewriteMode, nameof(RewriteMode));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("WrapTextSoundCol")]
    public CmdShortcut WrapTextSoundCol
    {
        get => _wrapTextSoundCol ??= InitShortcut(nameof(WrapTextSoundCol));
        set
        {
            _wrapTextSoundCol = value;
            AssignShortcutProps(ref _wrapTextSoundCol, nameof(WrapTextSoundCol));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("GoBack")]
    public CmdShortcut GoBack
    {
        get => _goBack ??= InitShortcut(nameof(GoBack));
        set
        {
            _goBack = value;
            AssignShortcutProps(ref _goBack, nameof(GoBack));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("GoForward")]
    public CmdShortcut GoForward
    {
        get => _goForward ??= InitShortcut(nameof(GoForward));
        set
        {
            _goForward = value;
            AssignShortcutProps(ref _goForward, nameof(GoForward));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Search")]
    public CmdShortcut Search
    {
        get => _search ??= InitShortcut(nameof(Search));
        set
        {
            _search = value;
            AssignShortcutProps(ref _search, nameof(Search));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("AddLang")]
    public CmdShortcut AddLanguage
    {
        get => _addLang ??= InitShortcut(nameof(AddLanguage));
        set
        {
            _addLang = value;
            AssignShortcutProps(ref _addLang, nameof(AddLanguage));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("EditLang")]
    public CmdShortcut EditLanguage
    {
        get => _editLang ??= InitShortcut(nameof(EditLanguage));
        set
        {
            _editLang = value;
            AssignShortcutProps(ref _editLang, nameof(EditLanguage));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("DeleteLang")]
    public CmdShortcut DeleteLanguage
    {
        get => _deleteLang ??= InitShortcut(nameof(DeleteLanguage));
        set
        {
            _deleteLang = value;
            AssignShortcutProps(ref _deleteLang, nameof(DeleteLanguage));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("AppSettings")]
    public CmdShortcut AppSettings
    {
        get => _appSettings ??= InitShortcut(nameof(AppSettings));
        set
        {
            _appSettings = value;
            AssignShortcutProps(ref _appSettings, nameof(AppSettings));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("HighlightProblem")]
    public CmdShortcut HighlightProblem
    {
        get => _highlightProblem ??= InitShortcut(nameof(HighlightProblem));
        set
        {
            _highlightProblem = value;
            AssignShortcutProps(ref _highlightProblem, nameof(HighlightProblem));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("ResolveProblem")]
    public CmdShortcut ResolveProblem
    {
        get => _resolveProblem ??= InitShortcut(nameof(ResolveProblem));
        set
        {
            _resolveProblem = value;
            AssignShortcutProps(ref _resolveProblem, nameof(ResolveProblem));
        }
    }

    #endregion

    public IList<CmdShortcut> GetValues()
    {
        var properties = ClassType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        return properties.Select(prop => (CmdShortcut)prop.GetValue(this)!).ToList();
    }

    public void SetValues(IEnumerable<CmdShortcut> shortcuts)
    {
        foreach (var shortcut in shortcuts) 
            ClassType.GetProperty(shortcut.PropertyName)?.SetValue(this, shortcut);
    }

    private static CmdShortcut InitShortcut(string propname) => new(Props[propname].shortcut, Props[propname].name, propname);

    private static void AssignShortcutProps(ref CmdShortcut obj, string propname)
    {
        if (obj is null)
        {
            obj = InitShortcut(propname);
        }
        else
        {
            obj.Name = Props[propname].name;
            obj.PropertyName = propname;
        }
    }

    // Every property setter below unconditionally assigns its backing field before this constructor
    // exits (see the "set" accessors above), but Roslyn's per-constructor flow analysis doesn't credit
    // assignment performed indirectly through a property setter call - it only sees `this` escaping into
    // a method call and forgets the field's null-state. All backing fields are genuinely never null here.
#pragma warning disable CS8618
    protected AppShortcuts(AppShortcuts original)
    {
        Open = original.Open with { };
        Save = original.Save with { };
        SaveAll = original.SaveAll with { };
        Undo = original.Undo with { };
        Redo = original.Redo with { };
        AddSound = original.AddSound with { };
        DeleteSounds = original.DeleteSounds with { };
        MoveSounds = original.MoveSounds with { };
        RewriteMode = original.RewriteMode with { };
        WrapTextSoundCol = original.WrapTextSoundCol with { };
        GoBack = original.GoBack with { };
        GoForward = original.GoForward with { };
        Search = original.Search with { };
        AddLanguage = original.AddLanguage with { };
        EditLanguage = original.EditLanguage with { };
        DeleteLanguage = original.DeleteLanguage with { };
        AppSettings = original.AppSettings with { };
        HighlightProblem = original.HighlightProblem with { };
        ResolveProblem = original.ResolveProblem with { };
    }
#pragma warning restore CS8618
}