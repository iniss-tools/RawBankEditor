using System.Reflection;
using System.Xml.Serialization;
using ToolsCore.XML;

namespace RawBankEditor.XML;

/// <summary>
///     Obsahuje zoznam všetkých možných stĺpcov pre tabuľku na pracovnej ploche programu
/// </summary>
public record DesktopColumns()
{
    private static readonly Type ClassType = typeof(DesktopColumns);

    [XmlIgnore]
    private static readonly Dictionary<string, (string name, int order, int minWidth, bool visible)> Props = new()
    {
        [nameof(Key)] = ("Kľúč", 0, 150, true),
        [nameof(Name)] = ("Názov", 1, 150, true),
        [nameof(RelativePath)] = ("Prídavná relatívna cesta", 2, 150, true),
        [nameof(FileName)] = ("Názov súboru", 3, 150, true),
        [nameof(Duration)] = ("Trvanie", 4, 100, true),
        [nameof(Text)] = ("Text hlásenia", 5, 200, true)
    };

    #region Fields

    private DesktopColumn _key = InitColumn(nameof(Key));
    private DesktopColumn _name = InitColumn(nameof(Name));
    private DesktopColumn _relativePath = InitColumn(nameof(RelativePath));
    private DesktopColumn _fileName = InitColumn(nameof(FileName));
    private DesktopColumn _duration = InitColumn(nameof(Duration));
    private DesktopColumn _text = InitColumn(nameof(Text));

    #endregion

    #region Properties

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Key")]
    public DesktopColumn Key
    {
        get => _key ??= InitColumn(nameof(Key));
        set
        {
            _key = value;
            AssignColumnProps(ref _key, nameof(Key));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Name")]
    public DesktopColumn Name
    {
        get => _name ??= InitColumn(nameof(Name));
        set
        {
            _name = value;
            AssignColumnProps(ref _name, nameof(Name));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("RelativePath")]
    public DesktopColumn RelativePath
    {
        get => _relativePath ??= InitColumn(nameof(RelativePath));
        set
        {
            _relativePath = value;
            AssignColumnProps(ref _relativePath, nameof(RelativePath));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("FileName")]
    public DesktopColumn FileName
    {
        get => _fileName ??= InitColumn(nameof(FileName));
        set
        {
            _fileName = value;
            AssignColumnProps(ref _fileName, nameof(FileName));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Duration")]
    public DesktopColumn Duration
    {
        get => _duration ??= InitColumn(nameof(Duration));
        set
        {
            _duration = value;
            AssignColumnProps(ref _duration, nameof(Duration));
        }
    }

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Text")]
    public DesktopColumn Text
    {
        get => _text ??= InitColumn(nameof(Text));
        set
        {
            _text = value;
            AssignColumnProps(ref _text, nameof(Text));
        }
    }

    #endregion

    /// <summary>
    ///     Vráti zoradený zoznam všetkých možných stĺpcov pre tabuľku na pracovnej ploche programu
    /// </summary>
    /// <returns></returns>
    public IList<DesktopColumn> GetValues()
    {
        var properties = ClassType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var ordered = properties.Select(prop => (DesktopColumn)prop.GetValue(this)!).ToList();
        return ordered.OrderBy(i => i.Order).ToList();
    }

    public void SetValues(IEnumerable<DesktopColumn> columns)
    {
        var index = 0;
        foreach (var column in columns)
        {
            column.Order = index++;
            ClassType.GetProperty(column.PropertyName)?.SetValue(this, column);
        }
    }

    private static DesktopColumn InitColumn(string propname)
        => new(Props[propname].name, propname, Props[propname].order, Props[propname].minWidth, Props[propname].visible);

    private static void AssignColumnProps(ref DesktopColumn obj, string propname)
    {
        if (obj is null)
        {
            obj = InitColumn(propname);
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
    protected DesktopColumns(DesktopColumns original)
    {
        Key = original.Key with { };
        Name = original.Name with { };
        RelativePath = original.RelativePath with { };
        FileName = original.FileName with { };
        Duration = original.Duration with { };
        Text = original.Text with { };
    }
#pragma warning restore CS8618
}