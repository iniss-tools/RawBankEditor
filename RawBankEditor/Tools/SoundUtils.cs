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
    private const int SOUND_ERROR = -2;

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
    ///     Converts .EWA encoded file to .WAV audio file.
    /// </summary>
    /// <param name="inpath">input file (.EWA)</param>
    /// <param name="outpath">output file (.WAV)</param>
    /// <param name="check">whether the format of .WAV file should be checked</param>
    public static void ConvertEWAtoWAV(string inpath, string outpath = null, bool check = false)
    {
        if (string.IsNullOrWhiteSpace(inpath))
            throw new ArgumentNullException(nameof(inpath));
        if (!File.Exists(inpath))
            throw new FileNotFoundException("Input file must be exists.", inpath);

        outpath ??= Path.ChangeExtension(inpath, WAV_EXT);

        using var inStream = new FileStream(inpath, FileMode.Open, FileAccess.Read);
        using var outStream = new FileStream(outpath, FileMode.Create, FileAccess.Write);
        using var reader = new BinaryReader(inStream);
        using var writer = new BinaryWriter(outStream);

        ConvertEWAtoWAV(reader, writer, check);
    }

    /// <summary>
    ///     Converts .EWA stream to .WAV stream.
    /// </summary>
    /// <param name="instream">input stream (.EWA)</param>
    /// <param name="outstream">output stream (.WAV)</param>
    /// <param name="check">whether the format of .WAV stream should be checked</param>
    public static void ConvertEWAtoWAV(BinaryReader instream, BinaryWriter outstream, bool check = false)
    {
        var b = instream.ReadByte();
        var ewaByte = (byte)(b ^ 82);
        instream.BaseStream.Position = 0;

        while (instream.BaseStream.Position < instream.BaseStream.Length)
        {
            var pos = instream.BaseStream.Position;
            b = instream.ReadByte();
            b = (byte)(b ^ ewaByte);
            b = (byte)((b - 0x11 * pos) & 0xff);
            outstream.Write(b);
        }

        if (check)
        {
            outstream.BaseStream.Position = 0;
            CheckWAV(instream);
        }
    }

    /// <summary>
    ///     Converts .WAV audio file to .EWA encoded file.
    /// </summary>
    /// <param name="inpath">input file (.WAV)</param>
    /// <param name="outpath">output file (.EWA)</param>
    /// <param name="check">whether the format of .WAV file should be checked</param>
    public static void ConvertWAVtoEWA(string inpath, string outpath = null, bool check = false)
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
    ///     Converts .EWA stream to .WAV stream.
    /// </summary>
    /// <param name="instream">input stream (.WAV)</param>
    /// <param name="outstream">output stream (.EWA)</param>
    /// <param name="check">whether the format of .WAV stream should be checked</param>
    public static void ConvertWAVtoEWA(BinaryReader instream, BinaryWriter outstream, bool check = false)
    {
        var rand = new Random();
        var ewaByte = (byte)rand.Next(0xff + 1);

        if (check)
        {
            instream.BaseStream.Position = 0;
            CheckWAV(instream);
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
    ///     Vrati dlzku zvuku (.WAV|.EWA) v milisekundach (ms)
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
        catch (FormatException)
        {
            Program.MainForm.Invoke(() =>
            {
                GlobData.OpenedProject.Messages[Program.MainForm.CurrentLanguage].Add(new InvalidSoundFile(file));
            });
            
            return SOUND_ERROR;
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

    public static void ConvertSoundsLanguage(FyzLanguage lang, bool toEwa, ToolStripProgressBar bar)
    {
        foreach (var group in lang.Groups) 
            ConvertSoundsInGroup(group, toEwa, bar);
    }

    public static void ConvertSoundsInGroup(FyzGroup grp, bool toEwa, ToolStripProgressBar bar)
    {
        ConvertSounds(grp.Sounds, toEwa, bar);
    }

    public static void ConvertSounds(IEnumerable<FyzSound> sounds, bool toEwa, ToolStripProgressBar bar)
    {
        var ext = toEwa ? EWA_EXT : WAV_EXT;
        foreach (var sound in sounds)
        {
            if (sound.File is null || sound.File.FileInfo.Extension.EqualsIgnoreCase(ext) || !File.Exists(sound.File.FileInfo.FullName))
            {
                bar.Owner.Invoke(() => bar.Increment(1));
                continue;
            }

            var newPath = Path.ChangeExtension(sound.File.FileInfo.FullName, ext);
            if (toEwa)
                ConvertWAVtoEWA(sound.File.FileInfo.FullName, newPath);
            else
                ConvertEWAtoWAV(sound.File.FileInfo.FullName, newPath);
            File.Delete(sound.File.FileInfo.FullName);
            sound.File.FileInfo = new FileInfo(newPath);
            sound.FileName = sound.File.FileInfo.Name;
            sound.File.Name = sound.FileName;
            bar.Owner.Invoke(() => bar.Increment(1));
        }
    }

    public static void ConvertFiles(IEnumerable<FileSystemElement> elements, bool toEwa)
    {
        var ext = toEwa ? EWA_EXT : WAV_EXT;
        foreach (var element in elements)
        {
            switch (element)
            {
                case SoundFileElement sfe:
                    if (sfe.FileInfo is null || sfe.FileInfo.Extension.EqualsIgnoreCase(ext) || File.Exists(sfe.FileInfo.FullName))
                        continue;

                    var newPath = Path.ChangeExtension(sfe.FileInfo.FullName, ext);
                    if (toEwa)
                        ConvertWAVtoEWA(sfe.FileInfo.FullName, newPath);
                    else
                        ConvertEWAtoWAV(sfe.FileInfo.FullName, newPath);
                    sfe.FileInfo = new FileInfo(newPath);
                    if (sfe.Sound is not null)
                    {
                        sfe.Sound.FileName = sfe.FileInfo.Name;
                    }
                    break;
                case DirectoryElement de:
                    ConvertFiles(de.Children, toEwa);
                    break;
            }
        }
    }
}