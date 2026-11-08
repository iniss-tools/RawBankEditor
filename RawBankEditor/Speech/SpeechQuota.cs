namespace RawBankEditor.Speech;

/// <summary>
/// Predplatne sluzby.
/// </summary>
/// <param name="Tier">nazov predplatneho</param>
/// <param name="Used">minute znaky v obdobi</param>
/// <param name="Limit">znaky v obdobi</param>
public sealed record SpeechQuota(string Tier, long Used, long Limit)
{
    /// <summary>
    /// Zostavajuce znaky.
    /// </summary>
    public long Remaining => Math.Max(0, Limit - Used);
}
