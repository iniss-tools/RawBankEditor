using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Forms;

/// <summary>
/// Riadok okna prepisu - zvuk, jeho text a prepis nahravky.
/// </summary>
public sealed class TranscriptionRow(FyzSound sound)
{
    /// <summary>Zvuk.</summary>
    [Browsable(false)]
    public FyzSound Sound { get; } = sound;

    /// <summary>Prevziat prepis ako text zvuku.</summary>
    public bool Accept { get; set; }

    /// <summary>Kluc skupiny.</summary>
    public string Group => Sound.Group?.Key ?? "";

    /// <summary>Kluc zvuku.</summary>
    public string Key => Sound.Key;

    /// <summary>Sucasny text zvuku.</summary>
    public string CurrentText { get; } = sound.Text?.Trim() ?? "";

    /// <summary>Prepis (upravitelny) - novy text zvuku.</summary>
    public string Transcript { get; set; } = "";

    /// <summary>Prepis od sluzby.</summary>
    [Browsable(false)]
    public string Heard { get; private set; } = "";

    /// <summary>Zhoda prepisu so sucasnym textom; <see langword="null" />, ak zvuk text nema alebo este nie je prepis.</summary>
    [Browsable(false)]
    public double? Match { get; private set; }

    /// <summary>Zhoda v percentach.</summary>
    public string MatchText => Match is { } match ? string.Format(CultureInfo.CurrentCulture, Resources.Match_Percent, match * 100) : "";

    /// <summary>Text zvuku sa nezhoduje s nahravkou.</summary>
    [Browsable(false)]
    public bool Mismatch => Match < TextMatch.Threshold;

    /// <summary>Stav.</summary>
    public string Status { get; set; } = "";

    /// <summary>Prave sa prepisuje.</summary>
    [Browsable(false)]
    public bool Busy { get; set; }

    /// <summary>
    /// Prevezme prepis: zvuk bez textu ho dostane (prevzatie zapnute), pri texte sa porovna - nezhodu treba posudit.
    /// </summary>
    public void SetHeard(string heard)
    {
        Heard = heard;
        Transcript = TextMatch.AsSoundText(heard);
        if (CurrentText.Length == 0)
        {
            Match = null;
            Accept = Transcript.Length > 0;
            Status = Transcript.Length > 0 ? Resources.FTranscribe_StatusNew : Resources.FTranscribe_StatusEmpty;
            return;
        }

        Match = TextMatch.Similarity(CurrentText, heard);
        Accept = false;
        Status = Mismatch ? Resources.FTranscribe_StatusMismatch : Resources.FTranscribe_StatusMatch;
    }
}
