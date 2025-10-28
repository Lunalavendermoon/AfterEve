public class Weak_Effect : Multiplier_Effects
{
    /// <summary>
    /// Damage from all source is multiplied by a %
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    /// <param name="defenseDecreasePercent"> take this percent of extra damage (must be > 1) </param>
    public Weak_Effect(float duration, float effectMultiplier) : base(duration, effectMultiplier)
    {
        // facts
        effectStat = Stat.BasicDefense;
        isDebuff = true;
        isIncremental = false;

        // store as a negative value so that DebuffComparer works properly
        effectRate = -effectMultiplier;
    }
}
