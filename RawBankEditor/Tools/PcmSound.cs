namespace RawBankEditor.Tools;

/// <summary>
/// Zvuk nahravky v pamati - vzorky mono v rozsahu -1 az 1.
/// </summary>
/// <param name="Samples">vzorky</param>
/// <param name="SampleRate">vzorkovacia frekvencia v Hz</param>
public sealed record PcmSound(float[] Samples, int SampleRate)
{
    /// <summary>
    /// Dlzka v milisekundach - rovnako ako ju pocita INISS z bloku data.
    /// </summary>
    public int DurationMs => (int)(Samples.LongLength * 1000 / SampleRate);
}
