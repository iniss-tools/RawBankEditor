using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Rec s casmi znakov textu.
/// </summary>
/// <param name="Sound">rec</param>
/// <param name="Timings">casy znakov v poradi textu</param>
public sealed record TimedSpeech(PcmSound Sound, IReadOnlyList<CharTiming> Timings);
