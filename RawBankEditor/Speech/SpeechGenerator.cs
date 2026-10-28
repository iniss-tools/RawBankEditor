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
    /// Text, ktory vo vysledku zaznie (pri nosnej vete len uryvok) - meranie tempa.
    /// </summary>
    public string SpokenText { get; init; } = "";

    /// <summary>
    /// Pri nosnej vete bez casov znakov text pred uryvkom vysloveny samostatne - jeho dlzka urci odhad zaciatku
    /// uryvku; <see langword="null" /> = casy znakov alebo uryvok na zaciatku vety.
    /// </summary>
    public SpeechRequest? Before { get; init; }

    /// <summary>
    /// Pri nosnej vete bez casov znakov text za uryvkom vysloveny samostatne - jeho dlzka urci odhad konca uryvku.
    /// </summary>
    public SpeechRequest? After { get; init; }

    /// <summary>
    /// Nosna veta sa striha podla odhadu (sluzba nevracia casy znakov).
    /// </summary>
    public bool Estimated => Before is not null || After is not null;

    /// <summary>
    /// Znaky, ktore sluzba zauctuje (text poziadavky; kontext pred a za uryvkom sa nevyslovi). Samostatne vyslovene
    /// texty pri odhade su spolocne pre celu skupinu a v cache - nepocitaju sa.
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
/// <param name="numbersLanguage">jazyk, v ktorom sa cisla prevedu na slova (<see cref="NumberWords" />); prazdny = cisla ostanu</param>
public sealed class SpeechGenerator(ISpeechProvider provider, SpeechCache cache, VoiceProfile profile, SoundAdjustOptions? adjust,
    IReadOnlyList<PronunciationRule>? pronunciation = null, string numbersLanguage = "")
{
    // okolie hranice uryvku, v ktorom sa hlada ticho na rez (pred zaciatkom a za koncom)
    private const int CUT_BEFORE_MS = 100;
    private const int CUT_AFTER_MS = 150;
    private const int CUT_INSIDE_MS = 10;
    private const int CUT_FADE_MS = 5;
    // okolie odhadnutej hranice - samostatne vysloveny text konci predlzenou slabikou, hranica vo vete byva skor
    private const int ESTIMATE_BEFORE_MS = 250;
    private const int ESTIMATE_AFTER_MS = 100;
    // uroven ticha pri hladani reci - od spicky nahravky
    private const double SPEECH_BELOW_PEAK_DB = 35;

    /// <summary>
    /// Vygeneruje nahravku textu bez intonacie skupiny.
    /// </summary>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    public Task<GeneratedSound> GenerateAsync(string text, uint? seed, CancellationToken cancel) => GenerateAsync(new SpeechInput(text), seed, cancel);

    /// <summary>
    /// Pripravi poziadavku: vyslovnost, interpunkcia a kontext alebo nosna veta podla intonacie.
    /// </summary>
    /// <param name="seed">seed namiesto seedu profilu</param>
    /// <param name="speedFactor">uprava rychlosti hlasu pre tento uryvok (zjednotenie tempa); odtlacok nemeni</param>
    public PreparedSpeech Prepare(SpeechInput input, uint? seed, double speedFactor = 1)
    {
        ArgumentNullException.ThrowIfNull(input);
        var voice = Math.Abs(speedFactor - 1) < 0.001 ? profile : WithSpeed(profile, speedFactor);

        var prosody = input.Prosody ?? new GroupProsody();
        var spoken = Speakable(prosody.Spoken(input.Text));
        var before = Speakable(prosody.Before);
        var after = Speakable(prosody.After);

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
                request = voice.ToRequest(spoken, seed) with { PreviousText = before, NextText = after };
                break;
            case ProsodyTechnique.Carrier:
                cutStart = before.Length == 0 ? 0 : before.Length + 1;
                cutLength = spoken.Length;
                request = voice.ToRequest(string.Join(' ', new[] { before, spoken, after }.Where(t => t.Length > 0)), seed);
                break;
            default:
                request = voice.ToRequest(spoken, seed);
                break;
        }

        var estimated = technique == ProsodyTechnique.Carrier && !provider.Features.HasFlag(SpeechFeatures.Timings);
        var key = technique == ProsodyTechnique.Carrier && !estimated ? request.CacheKey(provider.Id + ":timed") : request.CacheKey(provider.Id);
        var fingerprint = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join('|', profile.Fingerprint(), technique,
            before, after, spoken))))[..16];
        return new PreparedSpeech(request, technique, cutStart, cutLength, key, fingerprint)
        {
            // samostatne texty so seedom profilu - pri generovani znova ostavaju v cache
            Before = estimated && before.Length > 0 ? voice.ToRequest(before) : null,
            After = estimated && after.Length > 0 ? voice.ToRequest(after) : null,
            SpokenText = technique == ProsodyTechnique.Carrier ? request.Text.Substring(cutStart, cutLength) : request.Text
        };
    }

    /// <summary>
    /// Text tak, ako ho sluzba dostane: najprv slovnik vyslovnosti (vynimky maju prednost), potom cisla slovami.
    /// </summary>
    public string Speakable(string text) => NumberWords.Apply(Pronunciation.Apply(text, pronunciation), numbersLanguage).Trim();

    /// <summary>
    /// Hlas s rychlostou upravenou o <paramref name="factor" /> v rozsahu, ktory sluzba prijme.
    /// </summary>
    private VoiceProfile WithSpeed(VoiceProfile voice, double factor)
    {
        var (min, max) = SpeechProviders.SpeedRange(provider.Id);
        var changed = voice.Clone();
        changed.Speed = Math.Round(Math.Clamp(voice.Speed * factor, min, max), 2);
        return changed;
    }

    /// <summary>
    /// Vysledok poziadavky je v cache - generovanie nic nestoji.
    /// </summary>
    public bool IsCached(PreparedSpeech prepared)
    {
        ArgumentNullException.ThrowIfNull(prepared);
        if (prepared.Estimated)
            return cache.Contains(prepared.CacheKey) && new[] { prepared.Before, prepared.After }.OfType<SpeechRequest>().All(r => cache.Contains(r.CacheKey(provider.Id)));
        return prepared.Technique == ProsodyTechnique.Carrier ? cache.ContainsTimed(prepared.CacheKey) : cache.Contains(prepared.CacheKey);
    }

    /// <summary>
    /// Vygeneruje nahravku uryvku s intonaciou jeho skupiny.
    /// </summary>
    /// <param name="seed">seed namiesto seedu profilu (generovanie znova dava inu nahravku)</param>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    public Task<GeneratedSound> GenerateAsync(SpeechInput input, uint? seed, CancellationToken cancel) => GenerateAsync(input, seed, 1, cancel);

    /// <summary>
    /// Vygeneruje nahravku uryvku s intonaciou jeho skupiny a upravenou rychlostou.
    /// </summary>
    /// <param name="seed">seed namiesto seedu profilu (generovanie znova dava inu nahravku)</param>
    /// <param name="speedFactor">uprava rychlosti hlasu (zjednotenie tempa)</param>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    public async Task<GeneratedSound> GenerateAsync(SpeechInput input, uint? seed, double speedFactor, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentException.ThrowIfNullOrWhiteSpace(input.Text);

        var prepared = Prepare(input, seed, speedFactor);
        PcmSound raw;
        bool fromCache;
        if (prepared.Estimated)
        {
            (var full, fromCache) = await SynthesizeCachedAsync(prepared.Request, cancel).ConfigureAwait(false);
            var before = prepared.Before is { } b ? (await SynthesizeCachedAsync(b, cancel).ConfigureAwait(false)).Sound : null;
            var after = prepared.After is { } a ? (await SynthesizeCachedAsync(a, cancel).ConfigureAwait(false)).Sound : null;
            raw = ExtractEstimated(full, before, after);
        }
        else if (prepared.Technique == ProsodyTechnique.Carrier)
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
            (raw, fromCache) = await SynthesizeCachedAsync(prepared.Request, cancel).ConfigureAwait(false);
        }

        var sound = SoundProcessing.Resample(raw, SoundProcessing.BankSampleRate);
        if (adjust is not null)
            sound = SoundProcessing.Process(sound, adjust);
        return new GeneratedSound(sound, fromCache, fromCache ? 0 : prepared.Characters);
    }

    private async Task<(PcmSound Sound, bool FromCache)> SynthesizeCachedAsync(SpeechRequest request, CancellationToken cancel)
    {
        var key = request.CacheKey(provider.Id);
        if (cache.TryGet(key) is { } cached)
            return (cached, true);

        var sound = await provider.SynthesizeAsync(request, cancel).ConfigureAwait(false);
        cache.Put(key, sound);
        return (sound, false);
    }

    /// <summary>
    /// Vystrihne uryvok z nosnej vety bez casov znakov. Zaciatok uryvku je za recou o dlzke samostatne vysloveneho
    /// textu pred nim, koniec pred recou o dlzke samostatne vysloveneho textu za nim; rez ide do najtichsieho miesta
    /// v okoli odhadu (pred nim 250 ms, za nim 100 ms).
    /// </summary>
    /// <param name="full">nosna veta</param>
    /// <param name="before">text pred uryvkom samostatne; <see langword="null" /> = uryvok na zaciatku</param>
    /// <param name="after">text za uryvkom samostatne; <see langword="null" /> = uryvok na konci</param>
    /// <exception cref="InvalidDataException">nosna veta je ticho alebo by uryvok bol prazdny</exception>
    public static PcmSound ExtractEstimated(PcmSound full, PcmSound? before, PcmSound? after)
    {
        ArgumentNullException.ThrowIfNull(full);

        var (start, end) = SpeechBounds(full) ?? throw new InvalidDataException("Carrier sentence contains no speech.");
        var from = 0;
        var to = full.DurationMs;
        if (before is not null && SpeechBounds(before) is var (beforeStart, beforeEnd))
        {
            var estimate = start + (beforeEnd - beforeStart);
            from = SoundProcessing.QuietestPointMs(full, estimate - ESTIMATE_BEFORE_MS, estimate + ESTIMATE_AFTER_MS);
        }

        if (after is not null && SpeechBounds(after) is var (afterStart, afterEnd))
        {
            var estimate = end - (afterEnd - afterStart);
            to = SoundProcessing.QuietestPointMs(full, estimate - ESTIMATE_BEFORE_MS, estimate + ESTIMATE_AFTER_MS);
        }

        if (to - from < 2 * CUT_FADE_MS)
            throw new InvalidDataException("Estimated fragment is empty.");
        return SoundProcessing.Fade(SoundProcessing.Cut(full, from, to), CUT_FADE_MS);

        static (int Start, int End)? SpeechBounds(PcmSound sound) =>
            SoundProcessing.SpeechBoundsMs(sound, SoundProcessing.PeakDb(sound) - SPEECH_BELOW_PEAK_DB);
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
