using System.Globalization;
using System.Text.RegularExpressions;

namespace RawBankEditor.Speech;

/// <summary>
/// Cisla slovami pred synteza - slovencina a cestina. Sluzby citaju cisla zle (Multilingual v2 jazyk cisla nepozna
/// a cislice cita v roznych jazykoch, datum v zlom pade), slova vyslovia spravne.
/// <list type="bullet">
/// <item>datum <c>11. aprila</c> = radova cislovka v 2. pade, <c>11. april</c> v 1. pade; <c>15. 3. 2024</c> = den a mesiac v 2. pade, rok</item>
/// <item>cas <c>14:05</c> = <c>strnast nula pat</c></item>
/// <item>radova cislovka <c>3. kolaj</c> v 1. pade podla rodu nasledujuceho slova</item>
/// <item>cislo do 4 cislic zakladnou cislovkou (1 a 2 podla rodu nasledujuceho slova)</item>
/// <item>5 a viac cislic alebo cislo s nulou na zaciatku po cifrach (cislo vozna, telefonu)</item>
/// </list>
/// Pady okrem datumu sa z textu urcit nedaju - cislovky su v 1. pade; vynimky patria do textu na vyslovenie alebo do
/// slovnika vyslovnosti (ten sa uplatni skor).
/// </summary>
public static partial class NumberWords
{
    /// <summary>
    /// Jazyky, v ktorych sa cisla prevadzaju (ISO 639-1).
    /// </summary>
    public static IReadOnlyList<string> Languages { get; } = ["sk", "cs"];

    /// <summary>
    /// Prevod je pre jazyk k dispozicii.
    /// </summary>
    public static bool Supports(string languageCode) => Languages.Contains(languageCode, StringComparer.OrdinalIgnoreCase);

    // datum cislami, cas, radova cislovka pred slovom, cislo
    [GeneratedRegex(@"(?<!\d)(?<day>\d{1,2})\.\s?(?<month>\d{1,2})\.(?:\s?(?<year>\d{4})(?!\d))?|(?<!\d)(?<hour>\d{1,2}):(?<minute>\d{2})(?!\d)|(?<![\d,])(?<ordinal>\d{1,2})\.(?=\s+(?<next>\p{Ll}\p{L}*))|(?<![\d,])(?<number>\d+)(?!\d|,\d)(?:\s+(?<word>\p{L}+))?")]
    private static partial Regex NumberPattern();

    /// <summary>
    /// Text s cislami slovami; jazyk bez prevodu = text bez zmeny.
    /// </summary>
    /// <param name="text">text</param>
    /// <param name="languageCode">jazyk reci (ISO 639-1)</param>
    public static string Apply(string text, string languageCode)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (!Supports(languageCode) || !text.Any(char.IsAsciiDigit))
            return text;

        var language = Language.Of(languageCode);
        return NumberPattern().Replace(text, m => Replace(m, language));
    }

    private static string Replace(Match m, Language language)
    {
        if (m.Groups["day"].Success)
        {
            var day = Ordinal(int.Parse(m.Groups["day"].Value, CultureInfo.InvariantCulture), Form.Genitive, language);
            var month = Ordinal(int.Parse(m.Groups["month"].Value, CultureInfo.InvariantCulture), Form.Genitive, language);
            if (day is null || month is null)
                return m.Value;
            var year = m.Groups["year"].Success ? " " + Cardinal(int.Parse(m.Groups["year"].Value, CultureInfo.InvariantCulture), Gender.Masculine, language) : "";
            return $"{day} {month}{year}";
        }

        if (m.Groups["hour"].Success)
        {
            var hour = int.Parse(m.Groups["hour"].Value, CultureInfo.InvariantCulture);
            var minute = int.Parse(m.Groups["minute"].Value, CultureInfo.InvariantCulture);
            if (hour > 24 || minute > 59)
                return m.Value;
            // 14:05 = strnast nula pat, 14:00 = strnast nula nula
            var minutes = minute < 10 ? $"{language.Digits[0]} {language.Digits[minute]}" : Cardinal(minute, Gender.Feminine, language);
            return $"{Cardinal(hour, Gender.Feminine, language)} {minutes}";
        }

        if (m.Groups["ordinal"].Success)
        {
            var next = m.Groups["next"].Value;
            var number = int.Parse(m.Groups["ordinal"].Value, CultureInfo.InvariantCulture);
            var form = language.GenitiveMonths.Contains(next, StringComparer.OrdinalIgnoreCase) ? Form.Genitive
                : language.Months.Contains(next, StringComparer.OrdinalIgnoreCase) ? Form.Masculine
                : language.GenderOf(next, true) switch
                {
                    Gender.Feminine => Form.Feminine,
                    Gender.Neuter => Form.Neuter,
                    _ => Form.Masculine
                };
            // bodka za ordinalom sa zahodi, medzera a slovo ostanu (nie su sucastou zhody)
            return Ordinal(number, form, language) ?? m.Value;
        }

        var digits = m.Groups["number"].Value;
        var word = m.Groups["word"];
        var tail = word.Success ? m.Value[digits.Length..] : "";
        // cislo vozna, telefonu, vlaku s nulou na zaciatku - po cifrach
        if (digits.Length >= 5 || (digits.Length > 1 && digits[0] == '0'))
            return string.Join(' ', digits.Select(d => language.Digits[d - '0'])) + tail;

        // za cislom 2 az 4 je mnozne cislo - koncovka rod neurci, len zname slova
        var gender = word.Success ? language.GenderOf(word.Value, false) : language.DefaultGender;
        return Cardinal(int.Parse(digits, CultureInfo.InvariantCulture), gender, language) + tail;
    }

    /// <summary>
    /// Zakladna cislovka 0 az 9999; 1 a 2 podla rodu.
    /// </summary>
    internal static string Cardinal(int number, Gender gender, Language language)
    {
        if (number == 0)
            return language.Digits[0];

        var parts = new List<string>();
        if (number >= 1000)
            parts.Add(language.Thousands[number / 1000]);
        if (number % 1000 >= 100)
            parts.Add(language.Hundreds[number % 1000 / 100]);

        var rest = number % 100;
        if (rest > 0)
        {
            if (rest < 20)
            {
                parts.Add(rest <= 2 ? language.OneTwo(rest, gender) : language.Units[rest]);
            }
            else
            {
                var tens = language.Tens[rest / 10];
                parts.Add(rest % 10 == 0 ? tens : language.JoinTens(tens, language.Units[rest % 10]));
            }
        }

        return string.Join(' ', parts);
    }

    /// <summary>
    /// Radova cislovka 1 az 99 v tvare; <see langword="null" /> pre ine cislo (cita sa ako zakladna).
    /// </summary>
    internal static string? Ordinal(int number, Form form, Language language)
    {
        if (number is < 1 or > 99)
            return null;

        var words = number < 20 ? [language.OrdinalUnits[number]]
            : number % 10 == 0 ? [language.OrdinalTens[number / 10]]
            : new[] { language.OrdinalTens[number / 10], language.OrdinalUnits[number % 10] };
        return string.Join(' ', words.Select(w => language.Inflect(w, form)));
    }

    /// <summary>
    /// Rod podstatneho mena za cislom.
    /// </summary>
    internal enum Gender
    {
        Masculine,
        Feminine,
        Neuter
    }

    /// <summary>
    /// Tvar radovej cislovky.
    /// </summary>
    internal enum Form
    {
        Masculine,
        Feminine,
        Neuter,
        Genitive
    }

    /// <summary>
    /// Slova a pravidla jazyka.
    /// </summary>
    internal sealed class Language
    {
        public required string[] Digits { get; init; }
        public required string[] Units { get; init; }
        public required string[] Tens { get; init; }
        public required string[] Hundreds { get; init; }
        public required string[] Thousands { get; init; }
        public required string[] OrdinalUnits { get; init; }
        public required string[] OrdinalTens { get; init; }
        public required string[] Months { get; init; }
        public required string[] GenitiveMonths { get; init; }
        public required string[] FeminineNouns { get; init; }
        public required string[] NeuterNouns { get; init; }
        public required Gender DefaultGender { get; init; }
        public required Func<int, Gender, string> OneTwo { get; init; }
        public required Func<string, string, string> JoinTens { get; init; }
        public required Func<string, Form, string> Inflect { get; init; }
        public required Func<string, Gender> EndingGender { get; init; }

        /// <summary>
        /// Rod slova - zname slova zeleznice, inak podla koncovky (len jednotne cislo) alebo muzsky.
        /// </summary>
        /// <param name="word">slovo za cislom</param>
        /// <param name="byEnding">slovo je v jednotnom cisle (za radovou cislovkou) - rod sa da odhadnut z koncovky</param>
        public Gender GenderOf(string word, bool byEnding)
        {
            var lower = word.ToLowerInvariant();
            if (FeminineNouns.Any(n => lower.StartsWith(n, StringComparison.Ordinal)))
                return Gender.Feminine;
            if (NeuterNouns.Any(n => lower.StartsWith(n, StringComparison.Ordinal)))
                return Gender.Neuter;
            return byEnding ? EndingGender(lower) : Gender.Masculine;
        }

        public static Language Of(string code) => code.Equals("cs", StringComparison.OrdinalIgnoreCase) ? Czech : Slovak;

        private static readonly Language Slovak = new()
        {
            Digits = ["nula", "jedna", "dva", "tri", "štyri", "päť", "šesť", "sedem", "osem", "deväť"],
            Units =
            [
                "nula", "jeden", "dva", "tri", "štyri", "päť", "šesť", "sedem", "osem", "deväť", "desať", "jedenásť", "dvanásť", "trinásť", "štrnásť",
                "pätnásť", "šestnásť", "sedemnásť", "osemnásť", "devätnásť"
            ],
            Tens = ["", "", "dvadsať", "tridsať", "štyridsať", "päťdesiat", "šesťdesiat", "sedemdesiat", "osemdesiat", "deväťdesiat"],
            Hundreds = ["", "sto", "dvesto", "tristo", "štyristo", "päťsto", "šesťsto", "sedemsto", "osemsto", "deväťsto"],
            Thousands = ["", "tisíc", "dvetisíc", "tritisíc", "štyritisíc", "päťtisíc", "šesťtisíc", "sedemtisíc", "osemtisíc", "deväťtisíc"],
            OrdinalUnits =
            [
                "", "prvý", "druhý", "tretí", "štvrtý", "piaty", "šiesty", "siedmy", "ôsmy", "deviaty", "desiaty", "jedenásty", "dvanásty", "trinásty",
                "štrnásty", "pätnásty", "šestnásty", "sedemnásty", "osemnásty", "devätnásty"
            ],
            OrdinalTens = ["", "", "dvadsiaty", "tridsiaty", "štyridsiaty", "päťdesiaty", "šesťdesiaty", "sedemdesiaty", "osemdesiaty", "deväťdesiaty"],
            Months = ["január", "február", "marec", "apríl", "máj", "jún", "júl", "august", "september", "október", "november", "december"],
            GenitiveMonths = ["januára", "februára", "marca", "apríla", "mája", "júna", "júla", "augusta", "septembra", "októbra", "novembra", "decembra"],
            FeminineNouns = ["koľaj", "minút", "hodin", "tried", "stanic", "zastávk", "osob", "korun", "kapit"],
            NeuterNouns = ["nástupišt", "poschod", "miest", "kilometr"],
            DefaultGender = Gender.Masculine,
            OneTwo = (n, g) => n == 1
                ? g switch { Gender.Feminine => "jedna", Gender.Neuter => "jedno", _ => "jeden" }
                : g == Gender.Masculine ? "dva" : "dve",
            // dvadsatjeden - slovenska cislovka sa pise spolu
            JoinTens = (tens, unit) => tens + unit,
            Inflect = (word, form) =>
            {
                // treti: tretia, tretie, tretieho; prvy, piaty: prva/piata, prve/piate, prveho/piateho
                if (word.EndsWith('í'))
                    return form switch { Form.Feminine => word[..^1] + "ia", Form.Neuter => word[..^1] + "ie", Form.Genitive => word[..^1] + "ieho", _ => word };
                var stem = word[..^1];
                var longEnding = word.EndsWith('ý');
                return form switch
                {
                    Form.Feminine => stem + (longEnding ? "á" : "a"),
                    Form.Neuter => stem + (longEnding ? "é" : "e"),
                    Form.Genitive => stem + (longEnding ? "ého" : "eho"),
                    _ => word
                };
            },
            EndingGender = word => word.EndsWith('a') ? Gender.Feminine
                : word.EndsWith('o') || word.EndsWith("ie", StringComparison.Ordinal) || word.EndsWith('e') ? Gender.Neuter
                : Gender.Masculine
        };

        private static readonly Language Czech = new()
        {
            Digits = ["nula", "jedna", "dva", "tři", "čtyři", "pět", "šest", "sedm", "osm", "devět"],
            Units =
            [
                "nula", "jedna", "dva", "tři", "čtyři", "pět", "šest", "sedm", "osm", "devět", "deset", "jedenáct", "dvanáct", "třináct", "čtrnáct",
                "patnáct", "šestnáct", "sedmnáct", "osmnáct", "devatenáct"
            ],
            Tens = ["", "", "dvacet", "třicet", "čtyřicet", "padesát", "šedesát", "sedmdesát", "osmdesát", "devadesát"],
            Hundreds = ["", "sto", "dvě stě", "tři sta", "čtyři sta", "pět set", "šest set", "sedm set", "osm set", "devět set"],
            Thousands = ["", "tisíc", "dva tisíce", "tři tisíce", "čtyři tisíce", "pět tisíc", "šest tisíc", "sedm tisíc", "osm tisíc", "devět tisíc"],
            OrdinalUnits =
            [
                "", "první", "druhý", "třetí", "čtvrtý", "pátý", "šestý", "sedmý", "osmý", "devátý", "desátý", "jedenáctý", "dvanáctý", "třináctý",
                "čtrnáctý", "patnáctý", "šestnáctý", "sedmnáctý", "osmnáctý", "devatenáctý"
            ],
            OrdinalTens = ["", "", "dvacátý", "třicátý", "čtyřicátý", "padesátý", "šedesátý", "sedmdesátý", "osmdesátý", "devadesátý"],
            Months = ["leden", "únor", "březen", "duben", "květen", "červen", "červenec", "srpen", "září", "říjen", "listopad", "prosinec"],
            GenitiveMonths = ["ledna", "února", "března", "dubna", "května", "června", "července", "srpna", "září", "října", "listopadu", "prosince"],
            FeminineNouns = ["kolej", "minut", "hodin", "tříd", "stanic", "zastávk", "osob", "korun", "kapit"],
            NeuterNouns = ["nástupišt", "patr", "míst", "kilometr"],
            // samostatna cislica sa v cestine cita "jedna"
            DefaultGender = Gender.Feminine,
            OneTwo = (n, g) => n == 1
                ? g switch { Gender.Feminine => "jedna", Gender.Neuter => "jedno", _ => "jeden" }
                : g == Gender.Masculine ? "dva" : "dvě",
            // dvacet jedna - ceska cislovka sa pise oddelene
            JoinTens = (tens, unit) => tens + " " + unit,
            Inflect = (word, form) =>
            {
                // prvni, treti: prvni, prvniho; druhy: druha, druhe, druheho
                if (word.EndsWith('í'))
                    return form == Form.Genitive ? word + "ho" : word;
                var stem = word[..^1];
                return form switch { Form.Feminine => stem + "á", Form.Neuter => stem + "é", Form.Genitive => stem + "ého", _ => word };
            },
            EndingGender = word => word.EndsWith('a') ? Gender.Feminine
                : word.EndsWith('o') || word.EndsWith("ení", StringComparison.Ordinal) || word.EndsWith("iště", StringComparison.Ordinal) ? Gender.Neuter
                : Gender.Masculine
        };
    }
}
