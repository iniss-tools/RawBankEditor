using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Tools;

/// <summary>
/// Vysledok kontroly importovanej tabulky - co sa do jazyka prida. Ak su chyby, neprida sa nic.
/// </summary>
public sealed class SoundImportPlan
{
    /// <summary>
    /// Nove skupiny jazyka; ich zvuky su uz v <see cref="FyzGroup.Sounds" />.
    /// </summary>
    public List<FyzGroup> NewGroups { get; } = [];

    /// <summary>
    /// Nove zvuky existujucich skupin - este nepridane do <see cref="FyzGroup.Sounds" />.
    /// </summary>
    public List<FyzSound> SoundsForExistingGroups { get; } = [];

    /// <summary>
    /// Preskocene zvuky (kluc v skupine uz bol) v tvare skupina/zvuk.
    /// </summary>
    public List<string> Skipped { get; } = [];

    /// <summary>
    /// Chyby tabulky s cislom riadku.
    /// </summary>
    public List<string> Errors { get; } = [];

    /// <summary>
    /// Vsetky pridavane zvuky.
    /// </summary>
    public IEnumerable<FyzSound> AllSounds => NewGroups.SelectMany(g => g.Sounds).Concat(SoundsForExistingGroups);
}
