using System.Security.Cryptography;
using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Co sa s nahravkou robi.
/// </summary>
public enum SpeechOperation
{
    /// <summary>Nova nahravka z textu.</summary>
    Tts,

    /// <summary>Zmena hlasu povodnej nahravky - tempo a intonacia ostanu.</summary>
    ChangeVoice,

    /// <summary>Vycistenie povodnej nahravky od sumu.</summary>
    Isolate
}

/// <summary>
/// Vygenerovana nahravka.
/// </summary>
/// <param name="Sound">zvuk vo formate banky</param>
/// <param name="FromCache">vysledok bol v cache - sluzba sa nevolala</param>
/// <param name="Characters">znaky odoslane sluzbe (0 pri vysledku z cache)</param>
public sealed record GeneratedSound(PcmSound Sound, bool FromCache, int Characters);

/// <summary>
/// Pripravena poziadavka - co sa posle sluzbe, kolko znakov to stoji a ako sa z vysledku ziska uryvok.
/// </summary>
/// <param name="Request">poziadavka</param>
/// <param name="Technique">pouzity sposob (kontext bez podpory modelu = nosna veta)</param>
/// <param name="CutStart">pri nosnej vete zaciatok uryvku v texte poziadavky</param>
/// <param name="CutLength">pri nosnej vete dlzka uryvku v texte poziadavky</param>
/// <param name="CacheKey">kluc vysledku v cache</param>
/// <param name="Fingerprint">odtlacok hlasu, intonacie a vysloveneho textu bez seedu - rovnaky = rovnaky prejav</param>
public sealed record PreparedSpeech(SpeechRequest Request, ProsodyTechnique Technique, int CutStart, int CutLength, string CacheKey, string Fingerprint)
{
    /// <summary>
    /// Znaky, ktore sluzba zauctuje (text poziadavky; kontext pred a za uryvkom sa nevyslovi).
    /// </summary>
    public int Characters => Request.Text.Length;
}

/// <summary>
/// Generovanie nahravok hlasom jazyka: slovnik vyslovnosti, intonacia skupiny (kontext alebo nosna veta s vystrihnutim
/// uryvku), vysledok z cache alebo zo sluzby a uprava zvuku (ticho, hlasitost) ako v okne Upravit zvuk.
/// </summary>
/// <param name="provider">sluzba syntezy</param>
/// <param name="cache">cache vysledkov</param>
/// <param name="profile">hlas jazyka</param>
/// <param name="adjust">uprava zvuku; <see langword="null" /> = len format banky</param>
/// <param name="pronunciation">slovnik vyslovnosti jazyka</param>
public sealed class SpeechGenerator(ISpeechProvider provider, SpeechCache cache, VoiceProfile profile, SoundAdjustOptions? adjust,
    IReadOnlyList<PronunciationRule>? pronunciation = null)
{
    // okolie hranice uryvku, v ktorom sa hlada ticho na rez (pred zaciatkom a za koncom)
    private const int CUT_BEFORE_MS = 100;
    private const int CUT_AFTER_MS = 150;
    private const int CUT_INSIDE_MS = 10;
    private const int CUT_FADE_MS = 5;

    /// <summary>
    /// Vygeneruje nahravku textu bez intonacie skupiny.
    /// </summary>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    public Task<GeneratedSound> GenerateAsync(string text, uint? seed, CancellationToken cancel) => GenerateAsync(new SpeechInput(text), seed, cancel);

    /// <summary>
    /// Pripravi poziadavku: vyslovnost, interpunkcia a kontext alebo nosna veta podla intonacie.
    /// </summary>
    /// <param name="seed">seed namiesto seedu profilu</param>
    public PreparedSpeech Prepare(SpeechInput input, uint? seed)
    {
        ArgumentNullException.ThrowIfNull(input);

        var prosody = input.Prosody ?? new GroupProsody();
        var spoken = Pronunciation.Apply(prosody.Spoken(input.Text), pronunciation).Trim();
        var before = Pronunciation.Apply(prosody.Before, pronunciation).Trim();
        var after = Pronunciation.Apply(prosody.After, pronunciation).Trim();

        var technique = prosody.Technique;
        if (technique == ProsodyTechnique.Context && !provider.SupportsContext(profile.ModelId))
            technique = ProsodyTechnique.Carrier;
        if (technique != ProsodyTechnique.None && before.Length == 0 && after.Length == 0)
            technique = ProsodyTechnique.None;

        SpeechRequest request;
        int cutStart = 0, cutLength = 0;
        switch (technique)
        {
            case ProsodyTechnique.Context:
                request = profile.ToRequest(spoken, seed) with { PreviousText = before, NextText = after };
                break;
            case ProsodyTechnique.Carrier:
                cutStart = before.Length == 0 ? 0 : before.Length + 1;
                cutLength = spoken.Length;
                request = profile.ToRequest(string.Join(' ', new[] { before, spoken, after }.Where(t => t.Length > 0)), seed);
                break;
            default:
                request = profile.ToRequest(spoken, seed);
                break;
        }

        var key = technique == ProsodyTechnique.Carrier ? request.CacheKey(provider.Id + ":timed") : request.CacheKey(provider.Id);
        var fingerprint = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join('|', profile.Fingerprint(), technique,
            before, after, spoken))))[..16];
        return new PreparedSpeech(request, technique, cutStart, cutLength, key, fingerprint);
    }

    /// <summary>
    /// Vysledok poziadavky je v cache - generovanie nic nestoji.
    /// </summary>
    public bool IsCached(PreparedSpeech prepared)
    {
        ArgumentNullException.ThrowIfNull(prepared);
        return prepared.Technique == ProsodyTechnique.Carrier ? cache.ContainsTimed(prepared.CacheKey) : cache.Contains(prepared.CacheKey);
    }

    /// <summary>
    /// Vygeneruje nahravku uryvku s intonaciou jeho skupiny.
    /// </summary>
    /// <param name="seed">seed namiesto seedu profilu (generovanie znova dava inu nahravku)</param>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    public async Task<GeneratedSound> GenerateAsync(SpeechInput input, uint? seed, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentException.ThrowIfNullOrWhiteSpace(input.Text);

        var prepared = Prepare(input, seed);
        PcmSound raw;
        bool fromCache;
        if (prepared.Technique == ProsodyTechnique.Carrier)
        {
            var timed = cache.TryGetTimed(prepared.CacheKey);
            fromCache = timed is not null;
            if (timed is null)
            {
                timed = await provider.SynthesizeTimedAsync(prepared.Request, cancel).ConfigureAwait(false);
                cache.PutTimed(prepared.CacheKey, timed);
            }

            raw = Extract(timed, prepared.Request.Text, prepared.CutStart, prepared.CutLength);
        }
        else
        {
            var cached = cache.TryGet(prepared.CacheKey);
            fromCache = cached is not null;
            raw = cached ?? await provider.SynthesizeAsync(prepared.Request, cancel).ConfigureAwait(false);
            if (cached is null)
                cache.Put(prepared.CacheKey, raw);
        }

        var sound = SoundProcessing.Resample(raw, SoundProcessing.BankSampleRate);
        if (adjust is not null)
            sound = SoundProcessing.Process(sound, adjust);
        return new GeneratedSound(sound, fromCache, fromCache ? 0 : prepared.Characters);
    }

    /// <summary>
    /// Vystrihne uryvok z nosnej vety podla casov jeho znakov. Hranice sa posunu do najtichsieho miesta v okoli, aby rez
    /// neprejde cez slovo - pred uryvkom do 100 ms, za nim do 150 ms (pauza po ciarke ci bodke).
    /// </summary>
    /// <param name="timed">nosna veta s casmi znakov</param>
    /// <param name="text">text nosnej vety</param>
    /// <param name="start">zaciatok uryvku v texte</param>
    /// <param name="length">dlzka uryvku v texte</param>
    /// <exception cref="InvalidDataException">casy znakov nezodpovedaju textu</exception>
    public static PcmSound Extract(TimedSpeech timed, string text, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(timed);
        ArgumentNullException.ThrowIfNull(text);

        var timings = timed.Timings;
        // sluzba vracia casy po znakoch povodneho textu; ak sa pocet nezhoduje, uryvok sa hlada v jej znakoch
        if (timings.Count != text.Length)
        {
            var aligned = new string(timings.Select(t => t.Character).ToArray());
            var found = aligned.IndexOf(text.Substring(start, length), StringComparison.Ordinal);
            if (found < 0)
                throw new InvalidDataException("Alignment does not match the text.");
            start = found;
        }

        var chars = Enumerable.Range(start, length).Where(i => i < timings.Count && !char.IsWhiteSpace(timings[i].Character)).ToList();
        if (chars.Count == 0)
            throw new InvalidDataException("Alignment does not match the text.");

        var fromMs = (int)(timings[chars[0]].Start * 1000);
        var toMs = (int)(timings[chars[^1]].End * 1000);
        var cutFrom = start == 0 ? 0 : SoundProcessing.QuietestPointMs(timed.Sound, fromMs - CUT_BEFORE_MS, fromMs + CUT_INSIDE_MS);
        var cutTo = start + length >= text.Length
            ? timed.Sound.DurationMs
            : SoundProcessing.QuietestPointMs(timed.Sound, toMs - CUT_INSIDE_MS, toMs + CUT_AFTER_MS);
        return SoundProcessing.Fade(SoundProcessing.Cut(timed.Sound, cutFrom, Math.Max(cutTo, cutFrom + 1)), CUT_FADE_MS);
    }

    /// <summary>
    /// Odtlacok zmeny hlasu tymto hlasom (bez seedu) - nahravka uz zmenena tymto hlasom sa znova nemeni.
    /// </summary>
    public string ConversionFingerprint() =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join('|', profile.Fingerprint(), "sts", ElevenLabsClient.StsModel))))[..16];

    /// <summary>
    /// Zmeni hlas nahravky (<see cref="SpeechOperation.ChangeVoice" />) alebo ju vycisti od sumu
    /// (<see cref="SpeechOperation.Isolate" />). Vysledok sa uklada do cache podla obsahu nahravky.
    /// </summary>
    /// <param name="operation">operacia nad nahravkou</param>
    /// <param name="sourceFile">nahravka v banke (.WAV alebo .EWA)</param>
    /// <param name="seed">seed zmeny hlasu namiesto seedu profilu</param>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    /// <exception cref="InvalidDataException">nahravka sa neda precitat</exception>
    public async Task<GeneratedSound> ProcessAudioAsync(SpeechOperation operation, string sourceFile, uint? seed, CancellationToken cancel)
    {
        if (operation == SpeechOperation.Tts)
            throw new ArgumentOutOfRangeException(nameof(operation));

        // povodna nahravka v jednotnom formate - kluc cache nezavisi od .EWA kodovania ani hlavicky
        var source = SoundProcessing.ToWav(SoundProcessing.ReadFile(sourceFile));
        var conversion = new VoiceConversionRequest(source, profile.VoiceId, ElevenLabsClient.StsModel)
        {
            Stability = profile.Stability,
            SimilarityBoost = profile.SimilarityBoost,
            Style = profile.Style,
            SpeakerBoost = profile.SpeakerBoost,
            Seed = seed ?? profile.Seed
        };
        var key = operation == SpeechOperation.ChangeVoice
            ? conversion.CacheKey(provider.Id)
            : Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(provider.Id + "|isolate|" + Convert.ToHexStringLower(SHA256.HashData(source)))));

        var raw = cache.TryGet(key);
        var fromCache = raw is not null;
        if (raw is null)
        {
            raw = operation == SpeechOperation.ChangeVoice
                ? await provider.ConvertVoiceAsync(conversion, cancel).ConfigureAwait(false)
                : await provider.IsolateAsync(source, cancel).ConfigureAwait(false);
            cache.Put(key, raw);
        }

        var sound = SoundProcessing.Resample(raw, SoundProcessing.BankSampleRate);
        if (adjust is not null)
            sound = SoundProcessing.Process(sound, adjust);
        return new GeneratedSound(sound, fromCache, 0);
    }

    /// <summary>
    /// Nahodny seed pre generovanie znova.
    /// </summary>
    public static uint NewSeed() => (uint)Random.Shared.NextInt64(1, uint.MaxValue);
}
