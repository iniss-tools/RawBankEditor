namespace RawBankEditor.Speech;

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
