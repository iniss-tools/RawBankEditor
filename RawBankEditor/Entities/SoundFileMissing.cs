using System.Globalization;
using RawBankEditor.Forms;
using RawBankEditor.Properties;
using ToolsCore.Iniss.Entities;
// ReSharper disable MemberCanBePrivate.Global

namespace RawBankEditor.Entities;

public class SoundFileMissing : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "SoundFileMissing";

    /// <inheritdoc />
    public MessageType Type => MessageType.Error;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_SoundMissing, Sound.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_RemoveSound, Sound.Name);

    /// <inheritdoc />
    public string Path => Sound.GetAbsPath("");

    public FyzSound Sound { get; }

    public SoundFileMissing(FyzSound sound)
    {
        Sound = sound;
    }

    public void Resolve(FMain form)
    {
        var action = new FMain.RemovedSoundsAction(form, Sound);
        action.Apply();
        form.RegisterNewAction(action);
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        var index = form.CurrentLanguage!.Groups.IndexOf(Sound.Group);
        if (index == -1) 
            return;

        form.dgvGroups.ClearSelection();
        var i = form.SelectSound(Sound);
        if (i != 0 && i != -1)
        {
            form.DoNotChangeSoundsSelection = true;
            form.dgvSounds.Rows[0].Selected = false;
            form.DoNotChangeSoundsSelection = false;
        }
    }
}
