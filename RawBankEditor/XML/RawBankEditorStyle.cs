using ToolsCore.XML;

namespace RawBankEditor.XML;

/// <summary>
/// Farebna schema RawBankEditora (len ovladacie prvky).
/// </summary>
public class RawBankEditorStyle : Style
{
    /// <inheritdoc />
    protected override Style CreateEmpty() => new RawBankEditorStyle();
}
