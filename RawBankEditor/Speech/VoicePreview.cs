using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Ukazka navrhnuteho hlasu.
/// </summary>
/// <param name="GeneratedVoiceId">identifikator ukazky pre vytvorenie hlasu</param>
/// <param name="Sound">ukazka</param>
public sealed record VoicePreview(string GeneratedVoiceId, PcmSound Sound);
