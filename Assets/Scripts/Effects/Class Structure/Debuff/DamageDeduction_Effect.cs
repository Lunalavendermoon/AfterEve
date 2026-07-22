public class DamageDeduction_Effect : Multiplier_Effects
{
    /// <summary>
    /// Base damage is multiplied by a %
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    /// <param name="effectMultiplyAdditive"> take this percent of base damage (ex. 40% reduction -> effectMultiplyAdditive value is 0.6f) </param>
    public DamageDeduction_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        // facts
        effectStat = Stat.BaseDamage;
        isDebuff = true;
        isIncremental = false;

        // store as a negative value so that DebuffComparer works properly
        effectRate = -effectMultiplyAdditive;

        iconType = IconType.DebuffWeak;
    }
}
