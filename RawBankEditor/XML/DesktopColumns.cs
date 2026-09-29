using System.Reflection;
using System.Xml.Serialization;
using RawBankEditor.Properties;
using ToolsCore.XML;

namespace RawBankEditor.XML;

/// <summary>
/// Obsahuje zoznam všetkých možných stĺpcov pre tabuľku na pracovnej ploche programu
/// </summary>
public record DesktopColumns()
{
    private static readonly Type ClassType = typeof(DesktopColumns);

    [XmlIgnore]
    private static readonly Dictionary<string, (string name, int order, int minWidth, bool visible)> Props = new()
    {
        [nameof(Key)] = (Resources.Column_Key, 0, 150, true),
        [nameof(Name)] = (Resources.Column_Name, 1, 150, true),
        [nameof(RelativePath)] = (Resources.Column_RelativePath, 2, 150, true),
        [nameof(FileName)] = (Resources.Column_FileName, 3, 150, true),
        [nameof(Duration)] = (Resources.Column_Duration, 4, 100, true),
        [nameof(Text)] = (Resources.Column_Text, 5, 200, true)
    };

    #region Properties

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Key")]
    public DesktopColumn Key
    {
        get => field ??= InitColumn(nameof(Key));
        set
        {
            field = value;
            AssignColumnProps(ref field, nameof(Key));
        }
    } = InitColumn(nameof(Key));

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Name")]
    public DesktopColumn Name
    {
        get => field ??= InitColumn(nameof(Name));
        set
        {
            field = value;
            AssignColumnProps(ref field, nameof(Name));
        }
    } = InitColumn(nameof(Name));

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("RelativePath")]
    public DesktopColumn RelativePath
    {
        get => field ??= InitColumn(nameof(RelativePath));
        set
        {
            field = value;
            AssignColumnProps(ref field, nameof(RelativePath));
        }
    } = InitColumn(nameof(RelativePath));

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("FileName")]
    public DesktopColumn FileName
    {
        get => field ??= InitColumn(nameof(FileName));
        set
        {
            field = value;
            AssignColumnProps(ref field, nameof(FileName));
        }
    } = InitColumn(nameof(FileName));

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Duration")]
    public DesktopColumn Duration
    {
        get => field ??= InitColumn(nameof(Duration));
        set
        {
            field = value;
            AssignColumnProps(ref field, nameof(Duration));
        }
    } = InitColumn(nameof(Duration));

    /// <summary>
    ///     
    /// </summary>
    [XmlElement("Text")]
    public DesktopColumn Text
    {
        get => field ??= InitColumn(nameof(Text));
        set
        {
            field = value;
            AssignColumnProps(ref field, nameof(Text));
        }
    } = InitColumn(nameof(Text));

    #endregion

    /// <summary>
    /// Vráti zoradený zoznam všetkých možných stĺpcov pre tabuľku na pracovnej ploche programu
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

    private static void AssignColumnProps(ref DesktopColumn? obj, string propname)
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
    
    protected DesktopColumns(DesktopColumns original)
    {
        if (original.Key != null) Key = original.Key with { };
        if (original.Name != null) Name = original.Name with { };
        if (original.RelativePath != null) RelativePath = original.RelativePath with { };
        if (original.FileName != null) FileName = original.FileName with { };
        if (original.Duration != null) Duration = original.Duration with { };
        if (original.Text != null) Text = original.Text with { };
    }
}