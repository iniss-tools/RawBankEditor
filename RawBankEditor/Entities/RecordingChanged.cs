using System.Globalization;
using RawBankEditor.Forms;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
// ReSharper disable MemberCanBePrivate.Global

namespace RawBankEditor.Entities;

/// <summary>
/// Nahravka zvuku banky INISS2 nezodpoveda MD5 alebo dlzke v banke (vysledok kontroly nahravok).
/// </summary>
public class RecordingChanged(RecordingMismatch mismatch) : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "RecordingChanged";

    /// <inheritdoc />
    public MessageType Type => MessageType.Warning;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_RecordingChanged, Mismatch.Sound.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_UpdateRecordingData, Mismatch.Sound.Name);

    /// <inheritdoc />
    public string Path => Mismatch.Sound.GetAbsPath("");

    public RecordingMismatch Mismatch { get; } = mismatch;

    /// <inheritdoc />
    public void Resolve(FMain form) => form.ApplyRecordingData([Mismatch]);

    /// <inheritdoc />
    public void Show(FMain form) => new SoundFileMissing(Mismatch.Sound).Show(form);
}
