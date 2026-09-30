using ToolsCore.XML;
// ReSharper disable UnusedMember.Global

namespace RawBankEditor.XML;

/// <summary>
/// Trieda definujuca farby a pisma pre viacere prvky programu
/// </summary>
public record RawBankEditorStyle : Style
{
    /// <inheritdoc />
    public RawBankEditorStyle()
    {
    }

    /// <inheritdoc />
    public RawBankEditorStyle(Style original) : base(original)
    {
    }
    
    public new static RawBankEditorStyle DefaultLightStyle => new(Style.DefaultLightStyle);

    public new static RawBankEditorStyle DefaultDarkStyle => new(Style.DefaultDarkStyle);

    /// <inheritdoc />
    public override string ToString() => Name;
}