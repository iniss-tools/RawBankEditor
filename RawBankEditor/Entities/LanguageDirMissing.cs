using System.Globalization;
using RawBankEditor.Forms;
using RawBankEditor.Properties;
using ToolsCore.Iniss.Entities;
// ReSharper disable MemberCanBePrivate.Global

namespace RawBankEditor.Entities;

public class LanguageDirMissing : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "LanguageDirMissing";

    /// <inheritdoc />
    public MessageType Type => MessageType.Error;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_LanguageMissing, Language.Key);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_CreateDir, Language.RelativePath);

    /// <inheritdoc />
    public string Path => Language.GetAbsPath("");

    public FyzLanguage Language { get; }

    public LanguageDirMissing(FyzLanguage language)
    {
        Language = language;
    }

    public void Resolve(FMain form)
    {
        form.CreateLanguageDirectory(Language);
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        // do nothing
    }
}
