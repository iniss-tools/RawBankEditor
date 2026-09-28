using System.Media;
using NAudio.Wave;
using RawBankEditor.Entities;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Tools;

public static class SoundUtils
{
    public const string EWA_EXT = ".EWA";
    public const string WAV_EXT = ".WAV";

    // -1 = este nezistene
    private const int SOUND_UNKNOWN_LENGTH = -1;

    // -2 = chyba
    internal const int SOUND_ERROR = -2;

    public static void Play(string soundpath)
    {
        if (!File.Exists(soundpath))
            return;
        try
        {
            switch (Path.GetExtension(soundpath).ToUpper())
            {
                case EWA_EXT:
                {
                    using var fileStream = new FileStream(soundpath, FileMode.Open);
                    using var memoryStream = new MemoryStream();
                    using var reader = new BinaryReader(fileStream);
                    using var writer = new BinaryWriter(memoryStream);

                    ConvertEWAtoWAV(reader, writer);

                    memoryStream.Position = 0;
                    using var player = new SoundPlayer(memoryStream);
                
                    player.Play();
                    break;
                }
                case WAV_EXT:
                {
                    using var player = new SoundPlayer(soundpath);
                    player.Play();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Utils.ShowError(e.Message);
        }
    }

    /// <summary>
    /// Converts .EWA encoded file to .WAV audio file.
    /// </summary>
    /// <param name="inpath">input file (.EWA)</param>
    /// <param name="outpath">output file (.WAV)</param>
    /// <param name="check">whether the format of .WAV file should be checked</param>
    public static void ConvertEWAtoWAV(string inpath, string? outpath = null, bool check = false)
    {
        if (string.IsNullOrWhiteSpace(inpath))
            throw new ArgumentNullException(nameof(inpath));
        if (!File.Exists(inpath))
            throw new FileNotFoundException("Input file must be exists.", inpath);

        outpath ??= Path.ChangeExtension(inpath, WAV_EXT);

        using var inStream = new FileStream(inpath, FileMode.Open, FileAccess.Read);
        // ReadWrite - pri kontrole sa vystupny WAV cita spat
        using var outStream = new FileStream(outpath, FileMode.Create, FileAccess.ReadWrite);
        using var reader = new BinaryReader(inStream);
        using var writer = new BinaryWriter(outStream);

        ConvertEWAtoWAV(reader, writer, check);
    }

    /// <summary>
    /// Converts .EWA stream to .WAV stream.
    /// </summary>
    /// <param name="instream">input stream (.EWA)</param>
    /// <param name="outstream">output stream (.WAV)</param>
    /// <param name="check">whether the format of .WAV stream should be checked (output stream must be readable and seekable)</param>
    /// <exception cref="FormatException">vystup nie je platny WAV (len pri <paramref name="check" />)</exception>
    public static void ConvertEWAtoWAV(BinaryReader instream, BinaryWriter outstream, bool check = false)
    {
        var b = instream.ReadByte();
        var ewaByte = (byte)(b ^ 82);
        instream.BaseStream.Position = 0;

        while (instream.BaseStream.Position < instream.BaseStream.Length)
        {
            var pos = instream.BaseStream.Position;
            b = instream.ReadByte();
            // kluc 0 (prvy bajt 'R') - nekodovany WAV, INISS ho cita bez dekodovania
            if (ewaByte != 0)
            {
                b = (byte)(b ^ ewaByte);
                b = (byte)((b - 0x11 * pos) & 0xff);
            }
            outstream.Write(b);
        }

        if (check)
        {
            outstream.Flush();
            var end = outstream.BaseStream.Position;
            outstream.BaseStream.Position = 0;
            using (var outReader = new BinaryReader(outstream.BaseStream, Encoding.ASCII, true))
                CheckWAV(outReader);
            outstream.BaseStream.Position = end;
        }
    }

    /// <summary>
    /// Converts .WAV audio file to .EWA encoded file.
    /// </summary>
    /// <param name="inpath">input file (.WAV)</param>
    /// <param name="outpath">output file (.EWA)</param>
    /// <param name="check">whether the format of .WAV file should be checked</param>
    public static void ConvertWAVtoEWA(string inpath, string? outpath = null, bool check = false)
    {
        if (string.IsNullOrWhiteSpace(inpath))
            throw new ArgumentNullException(nameof(inpath));
        if (!File.Exists(inpath))
            throw new FileNotFoundException("Input file must be exists.", inpath);

        outpath ??= Path.ChangeExtension(inpath, EWA_EXT);

        using var inStream = new FileStream(inpath, FileMode.Open, FileAccess.Read);
        using var outStream = new FileStream(outpath, FileMode.Create, FileAccess.Write);
        using var reader = new BinaryReader(inStream);
        using var writer = new BinaryWriter(outStream);

        ConvertWAVtoEWA(reader, writer, check);
    }

    /// <summary>
    /// Converts .EWA stream to .WAV stream.
    /// </summary>
    /// <param name="instream">input stream (.WAV)</param>
    /// <param name="outstream">output stream (.EWA)</param>
    /// <param name="check">whether the format of .WAV stream should be checked</param>
    public static void ConvertWAVtoEWA(BinaryReader instream, BinaryWriter outstream, bool check = false)
    {
        // kluc 0 nie - INISS podla neho spozna nekodovany WAV (prvy bajt 'R') a subor by nedekodoval
        var ewaByte = (byte)Random.Shared.Next(1, 0xff + 1);

        if (check)
        {
            instream.BaseStream.Position = 0;
            CheckWAV(instream);
            instream.BaseStream.Position = 0;
        }

        while (instream.BaseStream.Position < instream.BaseStream.Length)
        {
            var pos = instream.BaseStream.Position;
            var b = instream.ReadByte();
            b = (byte)((b + 0x11 * pos) & 0xff);
            b = (byte)(b ^ ewaByte);
            outstream.Write(b);
        }
    }

    private static void CheckWAV(BinaryReader reader)
    {
        if (Read4ByteString(reader) != "RIFF")
            throw new FormatException("Chybný formát WAV súboru (chýba hlavička RIFF)!");
        if (reader.ReadInt32() != reader.BaseStream.Length - 8)
            throw new FormatException("Chybný formát WAV súboru (chybná dĺžka súboru)");
        if (Read4ByteString(reader) != "WAVE")
            throw new FormatException("Chybný formát WAV súboru (nie je typu WAVE)");

        double formatPos = -1;
        var fmtLen = 0;

        while (reader.BaseStream.Length > reader.BaseStream.Position)
        {
            if (Read4ByteString(reader) == "fmt ")
            {
                fmtLen = reader.ReadInt32();
                formatPos = reader.BaseStream.Position;
                break;
            }

            reader.BaseStream.Position += reader.ReadInt32();
        }

        if (formatPos < 0)
            throw new FormatException("Chybný formát WAV súboru (nenájdený formát)");
        if (fmtLen < 16)
            throw new FormatException("Chybný formát WAV súboru (fmt chunk má dĺžku menšiu ako 16 bytov)");

        var formatTag = reader.ReadInt16();
        reader.ReadInt16(); //channels
        reader.ReadInt32(); //sample per second
        reader.ReadInt32(); //average bytes per seconds
        reader.ReadInt16(); //block align
        reader.ReadInt16(); //bits per sample

        if (formatTag == -2)
        {
            reader.ReadInt16();
            reader.ReadInt16();
            reader.ReadInt32();
            var b = new Guid(reader.ReadBytes(16));
            if (new Guid("00000001-0000-0010-8000-00AA00389B71") != b)
                throw new FormatException("Chybný formát WAV súboru (neznámy subformát (GUID))");
        }
    }

    private static string Read4ByteString(BinaryReader reader)
    {
        var array = new byte[4];
        var read = reader.Read(array, 0, array.Length);
        return Encoding.ASCII.GetString(array, 0, read);
    }

    /// <summary>
    /// Vrati dlzku zvuku (.WAV|.EWA) v milisekundach (ms)
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<int> GetSoundDuration(SoundFileElement file)
    {
        CheckSoundDurationInput(file);

        try
        {
            var path = file.FileInfo.FullName;
            switch (Path.GetExtension(path).ToUpper())
            {
                case EWA_EXT:
                {
                    return await Task.Run(() =>
                    {
                        try
                        {
                            using var fileStream = new FileStream(path, FileMode.Open);
                            using var memoryStream = new MemoryStream();
                            using var reader = new BinaryReader(fileStream);
                            using var writer = new BinaryWriter(memoryStream);

                            ConvertEWAtoWAV(reader, writer);

                            memoryStream.Position = 0;
                            var wreader = new WaveFileReader(memoryStream);
                            var span = wreader.TotalTime;
                            return (int) span.TotalMilliseconds;
                        }
                        catch (IOException)
                        {
                            return SOUND_UNKNOWN_LENGTH;
                        }
                    });

                }
                case WAV_EXT:
                {
                    return await Task.Run(() =>
                    {
                        try
                        {
                            using var wreader = new WaveFileReader(path);
                            var span = wreader.TotalTime;
                            return (int) span.TotalMilliseconds;
                        }
                        catch (IOException)
                        {
                            return SOUND_UNKNOWN_LENGTH;
                        }
                    });
                }
                default:
                    return SOUND_ERROR;
            }
        }
        catch (Exception)
        {
            return SOUND_ERROR;
        }
    }

    private static void CheckSoundDurationInput(SoundFileElement file)
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));
    }

    /// <summary>
    /// Vysledok konverzie suborov.
    /// </summary>
    public sealed class ConvertResult
    {
        /// <summary>Skonvertovane subory - Spat ich skonvertuje naspat.</summary>
        public List<SoundFileElement> Converted { get; } = new();

        /// <summary>Subory, vedla ktorych uz subor s cielovou priponou je (neprepisuje sa).</summary>
        public List<string> Skipped { get; } = new();

        /// <summary>Subory, ktore sa skonvertovat nepodarilo, s popisom chyby.</summary>
        public List<string> Failed { get; } = new();
    }

    /// <summary>
    /// Subory nahravok vo vybranych prvkoch prieskumnika vratane obsahu priecinkov (rekurzivne).
    /// </summary>
    public static List<SoundFileElement> SoundFilesIn(IEnumerable<FileSystemElement> elements)
    {
        var files = new List<SoundFileElement>();
        foreach (var element in elements)
        {
            switch (element)
            {
                case SoundFileElement sfe:
                    files.Add(sfe);
                    break;
                case DirectoryElement de:
                    files.AddRange(SoundFilesIn(de.Children));
                    break;
            }
        }

        return files;
    }

    /// <summary>
    /// Skonvertuje subory (aj rekurzivne v priecinkoch) na .EWA alebo .WAV, pozri <see cref="ConvertSoundFiles" />.
    /// </summary>
    public static ConvertResult ConvertFiles(IEnumerable<FileSystemElement> elements, bool toEwa, System.Action? progress = null)
        => ConvertSoundFiles(SoundFilesIn(elements), toEwa, progress);

    /// <summary>
    /// Skonvertuje subory nahravok na .EWA alebo .WAV. Povodny subor sa zmaze a prvok aj priradeny zvuk
    /// dostanu novy nazov suboru.
    /// </summary>
    /// <remarks>
    /// Preskoci subory, ktore uz maju cielovu priponu alebo neexistuju, a subory, vedla ktorych uz cielovy
    /// subor existuje - ten moze patrit inemu zvuku, preto sa neprepisuje.
    /// </remarks>
    /// <param name="progress">Vola sa po kazdom subore.</param>
    public static ConvertResult ConvertSoundFiles(IEnumerable<SoundFileElement> files, bool toEwa, System.Action? progress = null)
    {
        var ext = toEwa ? EWA_EXT : WAV_EXT;
        var result = new ConvertResult();
        foreach (var sfe in files.Distinct())
        {
            var created = false;
            string? newPath = null;
            try
            {
                if (sfe.FileInfo is null || sfe.FileInfo.Extension.EqualsIgnoreCase(ext) || !File.Exists(sfe.FileInfo.FullName))
                    continue;

                var oldPath = sfe.FileInfo.FullName;
                newPath = Path.ChangeExtension(oldPath, ext);
                if (File.Exists(newPath))
                {
                    result.Skipped.Add(sfe.Name);
                    continue;
                }

                created = true;
                if (toEwa)
                    ConvertWAVtoEWA(oldPath, newPath);
                else
                    ConvertEWAtoWAV(oldPath, newPath);
                File.Delete(oldPath);
                created = false;

                sfe.FileInfo = new FileInfo(newPath);
                sfe.Name = sfe.FileInfo.Name;
                if (sfe.Sound is not null)
                    sfe.Sound.FileName = sfe.Name;
                result.Converted.Add(sfe);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                result.Failed.Add($"{sfe.Name}: {ex.Message}");
                // nedokonceny cielovy subor sa zmaze, povodny ostava
                if (created && newPath is not null && File.Exists(newPath) && File.Exists(sfe.FileInfo.FullName))
                    File.Delete(newPath);
            }
            finally
            {
                progress?.Invoke();
            }
        }

        return result;
    }

    /// <summary>
    /// Vrati predvoleny nazov suboru noveho zvuku podla kluca. Ak v priecinku skupiny existuje subor
    /// s nazvom kluca (.WAV/.EWA), pouzije sa jeho nazov, inak kluc s priponou prevladajucou v skupine
    /// (predvolene .WAV).
    /// </summary>
    public static string GetDefaultFileName(FyzGroup group, string key)
    {
        ArgumentNullException.ThrowIfNull(group);

        var existing = group.Directory?.Children
            .OfType<SoundFileElement>()
            .FirstOrDefault(f => RawBankExplorer.EqualsPathNames(Path.GetFileNameWithoutExtension(f.Name), key));
        if (existing is not null)
            return existing.Name;

        var ewaCount = group.Sounds.Count(s => s.FileName is not null && Path.GetExtension(s.FileName).EqualsIgnoreCase(EWA_EXT));
        return key + (ewaCount * 2 > group.Sounds.Count ? EWA_EXT : WAV_EXT);
    }

    /// <summary>
    /// Najde fyzicky subor zvuku rovnako ako <see cref="RawBankExplorer.MergeFilesAndData" />: pri prazdnej
    /// pridavnej ceste v priecinku skupiny podla nazvu suboru, inak na absolutnej ceste zvuku.
    /// </summary>
    /// <returns>prvok suboru alebo <c>null</c>, ak subor neexistuje.</returns>
    public static SoundFileElement? FindSoundFile(FyzSound sound, string pathToBank)
    {
        ArgumentNullException.ThrowIfNull(sound);

        if (!RawBankParser.AdditionalPathIsEmpty(sound.AdditionalRelativePath))
        {
            var path = sound.GetAbsPath(pathToBank);
            return File.Exists(path) ? new SoundFileElement(path) : null;
        }

        return sound.Group.Directory?.Children
            .OfType<SoundFileElement>()
            .FirstOrDefault(f => RawBankExplorer.EqualsPathNames(f.Name, sound.FileName));
    }
}