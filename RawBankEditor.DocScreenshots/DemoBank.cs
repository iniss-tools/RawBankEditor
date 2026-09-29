using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.DocScreenshots;

/// <summary>
/// Fiktívna inštalácia INISS so zvukovou bankou pre snímky do dokumentácie. Obsah zodpovedá ukážkovej banke
/// harnessu GVDEditora (stanica Dolné Mesto, stanice 99xxxxx), navyše s nahrávkami na disku: krátke
/// generované tóny s dĺžkou podľa textu, stanice a názvy vlakov vo formáte .EWA, ostatné .WAV.
/// </summary>
/// <remarks>
/// Banka má zámerne tri nezrovnalosti, aby zoznam chýb nebol prázdny: zvuk bez súboru, súbor bez zvuku
/// a prázdnu skupinu.
/// </remarks>
internal static class DemoBank
{
    public const string Marker = ".docshots";

    // stanice trate - kľúč zvuku je číslo stanice, text jej názov
    private static readonly (string Key, string Text)[] Stations =
    [
        ("9900010", "Veľká Ves"),
        ("9900020", "Horné Lúky"),
        ("9900030", "Brezová Dolina"),
        ("9900040", "Kamenný Brod"),
        ("9900050", "Nové Záhorie"),
        ("9900100", "Dolné Mesto"),
        ("9900110", "Lipová"),
        ("9900120", "Podhradie"),
        ("9900130", "Sklené Pole"),
        ("9900140", "Hraničná"),
    ];

    /// <summary>Zvuk, ktorý je v zozname, ale jeho súbor na disku chýba.</summary>
    public const string MissingFileKey = "nast";

    /// <summary>Súbor v priečinku stanice, ktorý v zozname zvukov nie je.</summary>
    public const string UndefinedFile = "9900150.WAV";

    /// <summary>Skupina bez zvukov.</summary>
    public const string EmptyGroupKey = "Reklama";

    /// <summary>
    /// Zostaví inštaláciu do <paramref name="root" /> (existujúci obsah vytvorený harnessom zmaže).
    /// </summary>
    public static void Build(string root, List<string> log)
    {
        // rovnaký priečinok a značku používa aj harness GVDEditora - skutočnú inštaláciu nikdy nezmaže
        var marker = Path.Combine(root, Marker);
        if (Directory.Exists(root))
        {
            if (!File.Exists(marker))
                throw new InvalidOperationException(
                    $"Priečinok {root} existuje a nevytvoril ho harness – nezmaže sa. Zadaj iný cez --work=….");
            Directory.Delete(root, true);
        }

        Directory.CreateDirectory(root);
        File.WriteAllText(marker, "Ukážková inštalácia INISS pre RawBankEditor.DocScreenshots – pri ďalšom spustení sa zmaže.");
        File.WriteAllBytes(Path.Combine(root, "INISS.exe"), []);

        var bankDir = Path.Combine(root, "RAWBANK");
        var sk = new FyzLanguage("SK", "Slovenčina", "FYZZVUK.DAT", @"SK\") { IsBasic = true };
        var gb = new FyzLanguage("GB", "Angličtina", "FYZZVUK.DAT", @"GB\");

        sk.Groups =
        [
            Group(sk, "R1", Stations, ewa: true),
            Group(sk, "V8", [("Brezovan", "Brezovan"), ("Lipovan", "Lipovan"), ("Podhradcan", "Podhradčan")], ewa: true),
            Group(sk, "Dodatky",
            [
                ("D1001", "Vlak je vedený náhradnou autobusovou dopravou."),
                ("D1002", "Na nástupišti prosíme dodržiavať bezpečnú vzdialenosť."),
            ]),
            Group(sk, "Poz1", [("0100", "koľaj 1"), ("0200", "koľaj 2"), ("0300", "koľaj 3"), ("0400", "koľaj 4")]),
            Group(sk, "Slova",
            [
                ("prich", "príde"), ("odch", "odíde"), ("kolaj", "na koľaj"), ("cislo", "číslo"),
                (MissingFileKey, "k nástupišťu"),
            ]),
            Group(sk, "Poz7", [("zalok", "Za rušňom sú radené:"), ("nakonci", "Na konci vlaku je radený")]),
            Group(sk, "VOZY1", [("v1", "vozeň prvej triedy"), ("rest", "reštauračný vozeň")]),
            Group(sk, "CISLO1", [("1", "jeden"), ("2", "dva")]),
            Group(sk, EmptyGroupKey, []),
        ];
        gb.Groups =
        [
            Group(gb, "R1", Stations),
            Group(gb, "Slova", [("arr", "arrives"), ("dep", "departs"), ("track", "at track")]),
        ];

        List<FyzLanguage> languages = [sk, gb];
        var files = 0;
        foreach (var language in languages)
        {
            Directory.CreateDirectory(Path.Combine(bankDir, language.RelativePath));
            foreach (var group in language.Groups)
            {
                var dir = Path.Combine(bankDir, language.RelativePath, group.RelativePath);
                Directory.CreateDirectory(dir);
                foreach (var sound in group.Sounds.Where(s => s.Key != MissingFileKey))
                {
                    WriteSound(Path.Combine(dir, sound.FileName), sound.Duration);
                    files++;
                }
            }

            RawBankParser.WriteFyzZvukFile(bankDir, language);
        }

        RawBankParser.WriteFyzBankFile(bankDir, languages);

        // nahrávka novej stanice nakopírovaná do banky bez záznamu v zozname
        WriteSound(Path.Combine(bankDir, "SK", "R1", UndefinedFile), 900);
        files++;

        log.Add($"demo: jazyky {languages.Count}, skupiny {languages.Sum(l => l.Groups.Count)}, " +
                $"zvuky {languages.Sum(l => l.Groups.Sum(g => g.Sounds.Count))}, súbory {files}");
    }

    private static FyzGroup Group(FyzLanguage language, string key, IEnumerable<(string Key, string Text)> sounds, bool ewa = false)
    {
        var group = new FyzGroup(language, key, key, key + @"\");
        foreach (var (soundKey, text) in sounds)
        {
            var file = soundKey + (ewa ? SoundUtils.EWAExt : SoundUtils.WAVExt);
            group.Sounds.Add(new FyzSound(group, soundKey, soundKey, file, "", text, DurationOf(text)));
        }

        return group;
    }

    /// <summary>
    /// Dĺžka nahrávky približne ako pri reči - podľa počtu znakov textu.
    /// </summary>
    private static int DurationOf(string text) => Math.Min(3500, 350 + 65 * text.Length);

    /// <summary>
    /// Tichý tón s nábehom a doznením (22 050 Hz, 16 bit, mono); .EWA vznikne rovnakou konverziou ako v programe.
    /// </summary>
    private static void WriteSound(string path, int durationMs)
    {
        const int rate = 22050;
        var samples = rate * durationMs / 1000;
        var wav = Path.ChangeExtension(path, SoundUtils.WAVExt);

        using (var writer = new BinaryWriter(File.Create(wav)))
        {
            writer.Write("RIFF"u8);
            writer.Write(36 + samples * 2);
            writer.Write("WAVE"u8);
            writer.Write("fmt "u8);
            writer.Write(16);
            writer.Write((short)1); // PCM
            writer.Write((short)1); // mono
            writer.Write(rate);
            writer.Write(rate * 2);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write("data"u8);
            writer.Write(samples * 2);

            for (var i = 0; i < samples; i++)
            {
                var envelope = Math.Min(1.0, Math.Min(i, samples - i) / (rate * 0.05));
                writer.Write((short)(Math.Sin(2 * Math.PI * 440 * i / rate) * 3000 * envelope));
            }
        }

        if (!Path.GetExtension(path).Equals(SoundUtils.EWAExt, StringComparison.OrdinalIgnoreCase))
            return;

        SoundUtils.ConvertWAVtoEWA(wav, path);
        File.Delete(wav);
    }
}
