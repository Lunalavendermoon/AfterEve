public class Weak_Effect : Multiplier_Effects
{
    /// <summary>
    /// Damage from all source is multiplied by a %
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    /// <param name="effectMultiplyAdditive"> take this percent of extra damage (ex. take 40% extra dmg -> effectMultiplyAdditive value is 0.4f) </param>
    public Weak_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        // facts
        effectStat = Stat.BasicDefense;
        isDebuff = true;
        isIncremental = false;

        // store as a negative value so that DebuffComparer works properly
        effectRate = -effectMultiplyAdditive;
    }
}
