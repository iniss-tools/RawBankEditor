namespace RawBankEditor.Speech;

/// <summary>
/// Model syntezy.
/// </summary>
/// <param name="Id">identifikator modelu</param>
/// <param name="Name">nazov</param>
/// <param name="Languages">kody jazykov, ktore model hovori (ISO 639-1)</param>
public sealed record SpeechModel(string Id, string Name, IReadOnlyList<string> Languages)
{
    /// <inheritdoc />
    public override string ToString() => Name.Length == 0 ? Id : $"{Name} ({Id})";
}
