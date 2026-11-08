using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Tools;

/// <summary>
/// Nahravka, ktora nezodpoveda udajom zvuku v banke INISS2.
/// </summary>
/// <param name="Sound">zvuk</param>
/// <param name="Md5">MD5 nahravky na disku</param>
/// <param name="Duration">dlzka nahravky na disku v milisekundach</param>
public sealed record RecordingMismatch(FyzSound Sound, string Md5, int Duration);
