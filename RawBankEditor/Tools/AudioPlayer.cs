using NAudio.Wave;

namespace RawBankEditor.Tools;

/// <summary>
/// Prehravanie nahravok (.WAV, .EWA aj WAV v pamati) cez NAudio. Nova nahravka zastavi predchadzajucu a prehravanie
/// sa da zastavit - na rozdiel od <see cref="System.Media.SoundPlayer" />.
/// </summary>
public sealed class AudioPlayer : IDisposable
{
    private WaveOutEvent? _output;
    private WaveFileReader? _reader;

    /// <summary>
    /// Prehravanie skoncilo alebo sa zastavilo (vola sa na vlakne, ktore prehravac vytvorilo - v okne na vlakne okna).
    /// </summary>
    public event EventHandler? Stopped;

    /// <summary>
    /// Prave sa nieco prehrava.
    /// </summary>
    public bool IsPlaying => _output?.PlaybackState == PlaybackState.Playing;

    /// <summary>
    /// Prehra subor nahravky; .EWA sa dekoduje v pamati.
    /// </summary>
    /// <exception cref="IOException">subor sa neda precitat</exception>
    /// <exception cref="InvalidDataException">subor nie je platny WAV</exception>
    public void PlayFile(string path) => Play(SoundProcessing.ReadWavBytes(path));

    /// <summary>
    /// Prehra WAV v pamati.
    /// </summary>
    /// <exception cref="InvalidDataException">obsah nie je platny WAV</exception>
    public void Play(byte[] wav)
    {
        ArgumentNullException.ThrowIfNull(wav);
        Stop();

        WaveFileReader reader;
        try
        {
            reader = new WaveFileReader(new MemoryStream(wav));
        }
        catch (FormatException e)
        {
            throw new InvalidDataException(e.Message, e);
        }

        var output = new WaveOutEvent();
        try
        {
            output.Init(reader);
        }
        catch
        {
            output.Dispose();
            reader.Dispose();
            throw;
        }

        output.PlaybackStopped += OnPlaybackStopped;
        _reader = reader;
        _output = output;
        output.Play();
    }

    /// <summary>
    /// Zastavi prehravanie.
    /// </summary>
    public void Stop()
    {
        var output = _output;
        if (output is null)
            return;

        // udalost by po zastaveni uvolnila uz dalsiu nahravku
        output.PlaybackStopped -= OnPlaybackStopped;
        output.Stop();
        Release();
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_output is not null)
            _output.PlaybackStopped -= OnPlaybackStopped;
        _output?.Stop();
        Release();
    }

    private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        if (!ReferenceEquals(sender, _output))
            return;

        Release();
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    private void Release()
    {
        _output?.Dispose();
        _reader?.Dispose();
        _output = null;
        _reader = null;
    }
}
