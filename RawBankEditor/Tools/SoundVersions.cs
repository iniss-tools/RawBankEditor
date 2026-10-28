using System.Globalization;

namespace RawBankEditor.Tools;

/// <summary>
/// Kopie povodnych a novych verzii nahravok pre Spat a Znova pri ich nahradeni (uprava zvuku, hlas AI). Zurnal banky
/// posiela povodne nahravky pri ulozeni do Kosa pod cestou zalohy - Spat po ulozeni by ich uz nenaslo. Kopie lezia
/// v docasnom priecinku procesu a po skonceni programu sa zmazu.
/// </summary>
public sealed class SoundVersions : IDisposable
{
    private const string PREFIX = "verzie-";
    private int _counter;

    /// <param name="root">priecinok kopii - zmaze sa pri <see cref="Dispose" /></param>
    public SoundVersions(string root)
    {
        Root = root;
    }

    /// <summary>
    /// Priecinok kopii.
    /// </summary>
    public string Root { get; }

    /// <summary>
    /// Kopie tohto procesu v docasnom priecinku; priecinky ukoncenych procesov sa zmazu.
    /// </summary>
    public static SoundVersions ForProcess()
    {
        var parent = Path.Combine(Path.GetTempPath(), "RawBankEditor");
        DeleteLeftovers(parent);
        return new SoundVersions(Path.Combine(parent, PREFIX + Environment.ProcessId.ToString(CultureInfo.InvariantCulture)));
    }

    /// <summary>
    /// Ulozi WAV a vrati cestu ku kopii.
    /// </summary>
    public string Add(byte[] wav)
    {
        var path = NewPath();
        File.WriteAllBytes(path, wav);
        return path;
    }

    /// <summary>
    /// Ulozi kopiu nahravky ako WAV (.EWA sa dekoduje) a vrati cestu ku kopii.
    /// </summary>
    public string AddCopyOf(string soundFile) => Add(SoundProcessing.ReadWavBytes(soundFile));

    /// <inheritdoc />
    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Root))
                Directory.Delete(Root, true);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            // docasny priecinok sa zmaze pri dalsom spusteni
        }
    }

    private string NewPath()
    {
        Directory.CreateDirectory(Root);
        return Path.Combine(Root, Interlocked.Increment(ref _counter).ToString(CultureInfo.InvariantCulture) + SoundUtils.WAVExt);
    }

    private static void DeleteLeftovers(string parent)
    {
        if (!Directory.Exists(parent))
            return;

        foreach (var dir in Directory.EnumerateDirectories(parent, PREFIX + "*"))
        {
            if (int.TryParse(Path.GetFileName(dir)[PREFIX.Length..], NumberStyles.None, CultureInfo.InvariantCulture, out var pid) && IsRunning(pid))
                continue;

            try
            {
                Directory.Delete(dir, true);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                // priecinok drzi iny proces - skusi sa nabuduce
            }
        }
    }

    private static bool IsRunning(int pid)
    {
        if (pid == Environment.ProcessId)
            return true;

        try
        {
            using var process = Process.GetProcessById(pid);
            return !process.HasExited;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
