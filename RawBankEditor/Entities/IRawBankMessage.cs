using System.Diagnostics.CodeAnalysis;
using RawBankEditor.Forms;
// ReSharper disable MemberCanBePrivate.Global

namespace RawBankEditor.Entities;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public interface IRawBankMessage
{
    public abstract string Code { get; }

    public abstract MessageType Type { get; }

    public abstract string Message { get; }

    public abstract string ResolveMessage { get; }

    public abstract string Path { get; }

    /// <summary>
    /// Vyriesi problem (Vyriesit v zozname chyb).
    /// </summary>
    /// <param name="form">hlavne okno</param>
    public abstract void Resolve(FMain form);

    /// <summary>
    /// Ukaze problem v hlavnom okne (vyberie skupinu, zvuk alebo subor).
    /// </summary>
    /// <param name="form">hlavne okno</param>
    public abstract void Show(FMain form);
}
