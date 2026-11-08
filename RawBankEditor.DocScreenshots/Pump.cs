namespace RawBankEditor.DocScreenshots;

/// <summary>
/// Spracovanie správ okien medzi krokmi, kým harness nemá vlastnú slučku správ.
/// </summary>
internal static class Pump
{
    public static void Events(int rounds = 6)
    {
        for (var i = 0; i < rounds; i++)
        {
            Application.DoEvents();
            Thread.Sleep(15);
        }
    }

    /// <summary>
    /// Spracúva správy, kým neplatí podmienka (napr. kým sa na pozadí nenačíta grafikon).
    /// </summary>
    public static bool Until(Func<bool> condition, int timeoutMs = 30000)
    {
        var start = Environment.TickCount64;
        while (!condition())
        {
            if (Environment.TickCount64 - start > timeoutMs)
                return false;
            Events(1);
        }

        Events();
        return true;
    }
}
