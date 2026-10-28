using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Tempo reci vygenerovanych nahravok - slabiky za sekundu znenia reci (bez prestavok). Sluzby menia tempo podla dlzky
/// textu (kratky uryvok zrychlia, dlhu vetu spomalia), hoci rychlost hlasu je rovnaka; uryvky hlasenia vsak maju zniet
/// rovnako. Porovnava sa s medianom nahravok v okne.
/// </summary>
public static class SpeechTempo
{
    /// <summary>Odchylka od medianu, od ktorej sa tempo zjednoti.</summary>
    public const double Tolerance = 0.1;

    /// <summary>Tempo pomalsie ako tento podiel medianu je podozrive.</summary>
    public const double SuspiciousSlow = 0.8;

    /// <summary>Tempo rychlejsie ako tento podiel medianu je podozrive.</summary>
    public const double SuspiciousFast = 1.2;

    /// <summary>Najmenej nahravok s tempom, aby mal median zmysel.</summary>
    public const int MinRows = 3;

    // samohlasky slovenciny, cestiny, anglictiny a nemciny - susedne tvoria jednu slabiku (dvojhlasky)
    private const string VOWELS = "aáäeéěiíoóôöuúůüyý";
    // slabicne r a l (vlk, prst, Brno)
    private const string LIQUIDS = "rŕlĺ";
    // cislica sa cita ako slovo - priblizne slabiky na cislicu
    private const int SYLLABLES_PER_DIGIT = 2;
    // kratky uryvok ma tempo prilis nahodne (predlzena posledna slabika je velka cast nahravky)
    private const int MIN_SYLLABLES = 5;
    private const int MIN_SPEECH_MS = 600;
    // rec je vsetko do 30 dB pod spickou - aj tichsie spoluhlasky
    private const double SPEECH_BELOW_PEAK_DB = 30;
    // jedna uprava rychlosti najviac o tretinu - sluzba nereaguje presne umerne
    private const double MAX_STEP = 1.33;

    /// <summary>
    /// Slabiky za sekundu znenia reci; <see langword="null" />, ak je text alebo rec na meranie prilis kratka.
    /// </summary>
    /// <param name="sound">nahravka</param>
    /// <param name="text">text, ktory v nahravke zaznie</param>
    public static double? Rate(PcmSound sound, string text)
    {
        ArgumentNullException.ThrowIfNull(sound);
        ArgumentNullException.ThrowIfNull(text);

        var syllables = Syllables(text);
        var ms = SoundProcessing.ActiveSpeechMs(sound, SoundProcessing.PeakDb(sound) - SPEECH_BELOW_PEAK_DB);
        return syllables < MIN_SYLLABLES || ms < MIN_SPEECH_MS ? null : syllables * 1000.0 / ms;
    }

    /// <summary>
    /// Priblizny pocet slabik: skupiny samohlasok, slabicne r a l medzi spoluhlaskami, skratka z velkych pismen po
    /// pismenach, cislica ako dve slabiky.
    /// </summary>
    public static int Syllables(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var count = 0;
        foreach (var word in text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            count += word.Count(char.IsDigit) * SYLLABLES_PER_DIGIT;
            var letters = new string(word.Where(char.IsLetter).ToArray());
            if (letters.Length == 0)
                continue;

            var lower = letters.ToLowerInvariant();
            var syllables = 0;
            for (var i = 0; i < lower.Length; i++)
            {
                var vowel = VOWELS.Contains(lower[i], StringComparison.Ordinal);
                if (vowel && (i == 0 || !VOWELS.Contains(lower[i - 1], StringComparison.Ordinal)))
                    syllables++;
                else if (!vowel && LIQUIDS.Contains(lower[i], StringComparison.Ordinal) && i > 0 && !VOWELS.Contains(lower[i - 1], StringComparison.Ordinal)
                         && (i == lower.Length - 1 || !VOWELS.Contains(lower[i + 1], StringComparison.Ordinal)))
                    syllables++;
            }

            // skratka (ZSSK, S) sa cita po pismenach; slovo bez samohlasky aspon jedna slabika
            if (letters.Length <= 5 && letters.All(char.IsUpper) && syllables < letters.Length)
                syllables = letters.Length;
            count += Math.Max(syllables, 1);
        }

        return count;
    }

    /// <summary>
    /// Median temp; <see langword="null" />, ak ich je menej nez <see cref="MinRows" />.
    /// </summary>
    public static double? Median(IEnumerable<double> rates)
    {
        var sorted = rates.Order().ToList();
        if (sorted.Count < MinRows)
            return null;
        var middle = sorted.Count / 2;
        return sorted.Count % 2 == 1 ? sorted[middle] : (sorted[middle - 1] + sorted[middle]) / 2;
    }

    /// <summary>
    /// Tempo je od medianu dalej nez <see cref="Tolerance" /> - zjednotenie ho upravi.
    /// </summary>
    /// <param name="ratio">tempo / median</param>
    public static bool NeedsCorrection(double ratio) => Math.Abs(ratio - 1) > Tolerance;

    /// <summary>
    /// Nova uprava rychlosti hlasu pre uryvok, ktory vysiel <paramref name="ratio" />-krat rychlejsie ako median.
    /// </summary>
    /// <param name="factor">doterajsia uprava</param>
    /// <param name="ratio">tempo / median</param>
    public static double Corrected(double factor, double ratio) => factor / Math.Clamp(ratio, 1 / MAX_STEP, MAX_STEP);
}
