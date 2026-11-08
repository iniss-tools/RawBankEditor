namespace RawBankEditor.DocScreenshots;

/// <summary>
/// Výška tónu podľa textu - nahrávky ukážkovej banky aj „reč“ <see cref="DemoSpeech" /> sú tóny, z ktorých výšky
/// prepis zistí text.
/// </summary>
internal static class DemoTones
{
    private const double BASE = 160;
    private const double STEP = 20;
    private static readonly List<string> Texts = [];

    public static double Frequency(string text)
    {
        lock (Texts)
        {
            var id = Texts.IndexOf(text);
            if (id < 0)
            {
                id = Texts.Count;
                Texts.Add(text);
            }

            return BASE + STEP * id;
        }
    }

    public static string? TextOf(double frequency)
    {
        var id = (int)Math.Round((frequency - BASE) / STEP);
        lock (Texts)
            return id >= 0 && id < Texts.Count ? Texts[id] : null;
    }
}
