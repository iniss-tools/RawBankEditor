using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RawBankEditor.DocScreenshots;

/// <summary>
/// Snímka okna tak, ako ho vykreslí Windows – vrátane (tmavého) titulku, bez neviditeľných okrajov
/// a bez zaoblených rohov. DrawToBitmap by titulok vykreslil v klasickom vzhľade a Scintillu vynechal.
/// </summary>
internal static partial class WindowCapture
{
    private const uint PwRenderFullContent = 2;
    private const int DwmwaExtendedFrameBounds = 9;
    private const int DwmwaWindowCornerPreference = 33;
    private const int DwmwcpDoNotRound = 1;

    public static void Save(Form form, string file)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        using var bitmap = Capture(form);
        bitmap.Save(file, ImageFormat.Png);
    }

    private static Bitmap Capture(Form form)
    {
        var hwnd = form.Handle;
        var corner = DwmwcpDoNotRound;
        _ = DwmSetWindowAttribute(hwnd, DwmwaWindowCornerPreference, ref corner, sizeof(int));
        Pump.Events();

        GetWindowRect(hwnd, out var window);
        if (DwmGetWindowAttribute(hwnd, DwmwaExtendedFrameBounds, out var frame, Marshal.SizeOf<Rect>()) != 0)
            frame = window;

        using var full = new Bitmap(window.Width, window.Height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(full))
        {
            var hdc = g.GetHdc();
            var ok = PrintWindow(hwnd, hdc, PwRenderFullContent);
            g.ReleaseHdc(hdc);
            if (!ok)
                form.DrawToBitmap(full, new Rectangle(0, 0, window.Width, window.Height));
        }

        // orezanie na viditeľný rám okna
        var crop = new Rectangle(frame.Left - window.Left, frame.Top - window.Top, frame.Width, frame.Height);
        crop.Intersect(new Rectangle(0, 0, full.Width, full.Height));
        return full.Clone(crop, PixelFormat.Format24bppRgb);
    }

    /// <summary>
    /// Titulky viditeľných okien tohto procesu – pri zaseknutí ukážu, ktoré modálne okno čaká.
    /// </summary>
    public static List<string> ProcessWindowTitles()
    {
        var pid = (uint)Environment.ProcessId;
        var titles = new List<string>();
        EnumWindows((hwnd, _param) =>
        {
            _ = GetWindowThreadProcessId(hwnd, out var owner);
            if (owner == pid && IsWindowVisible(hwnd))
            {
                var buffer = new char[256];
                var length = GetWindowText(hwnd, buffer, buffer.Length);
                titles.Add(new string(buffer, 0, length));
            }
            return true;
        }, IntPtr.Zero);
        return titles;
    }

    private delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr param);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr param);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hwnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hwnd, char[] text, int maxCount);

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left, Top, Right, Bottom;
        public readonly int Width => Right - Left;
        public readonly int Height => Bottom - Top;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint flags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetWindowRect(IntPtr hwnd, out Rect rect);

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmGetWindowAttribute(IntPtr hwnd, int attribute, out Rect value, int size);

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}

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
