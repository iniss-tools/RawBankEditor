using RawBankEditor.Speech;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Forms;

/// <summary>
/// Riadok okna intonacie - skupina a jej nastavenie.
/// </summary>
public sealed class ProsodyRow
{
    /// <param name="group">skupina</param>
    /// <param name="defaults">predvolena intonacia skupiny</param>
    /// <param name="saved">ulozena intonacia; <see langword="null" /> = predvolena</param>
    public ProsodyRow(FyzGroup group, GroupProsody defaults, GroupProsody? saved)
    {
        Group = group;
        Default = defaults;
        Load(saved ?? defaults);
    }

    /// <summary>Skupina.</summary>
    [Browsable(false)]
    public FyzGroup Group { get; }

    /// <summary>Predvolena intonacia.</summary>
    [Browsable(false)]
    public GroupProsody Default { get; }

    /// <summary>Kluc skupiny.</summary>
    public string GroupKey => Group.Key;

    /// <summary>Typ skupiny s popisom.</summary>
    public string TypeName => Group.Type.Description.Length == 0 ? Group.Type.Name : Group.Type.Description;

    /// <summary>Sposob.</summary>
    public ProsodyTechnique Technique { get; set; }

    /// <summary>Interpunkcia.</summary>
    public ProsodyPunctuation Punctuation { get; set; }

    /// <summary>Text pred uryvkom.</summary>
    public string Before { get; set; } = "";

    /// <summary>Text za uryvkom.</summary>
    public string After { get; set; } = "";

    /// <summary>
    /// Prevezme nastavenie.
    /// </summary>
    public void Load(GroupProsody prosody)
    {
        Technique = prosody.Technique;
        Punctuation = prosody.Punctuation;
        Before = prosody.Before;
        After = prosody.After;
    }

    /// <summary>
    /// Nastavenie riadku.
    /// </summary>
    public GroupProsody ToProsody() => new()
    {
        Technique = Technique,
        Punctuation = Punctuation,
        Before = Before?.Trim() ?? "",
        After = After?.Trim() ?? ""
    };
}
