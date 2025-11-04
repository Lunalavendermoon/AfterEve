public class Cursed_Effect : Multiplier_Effects
{
    /// <summary>
    /// Spiritual Defense is reduced by a %
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    /// <param name="defenseDecreasePercent"> lose this percent of Spiritual Defense </param>
    public Cursed_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        // facts
        effectStat = Stat.SpiritualDefense;
        isDebuff = true;
        isIncremental = false;
    }
}
