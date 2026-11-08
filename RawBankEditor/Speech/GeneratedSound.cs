using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Vygenerovana nahravka.
/// </summary>
/// <param name="Sound">zvuk vo formate banky</param>
/// <param name="FromCache">vysledok bol v cache - sluzba sa nevolala</param>
/// <param name="Characters">znaky odoslane sluzbe (0 pri vysledku z cache)</param>
public sealed record GeneratedSound(PcmSound Sound, bool FromCache, int Characters);
