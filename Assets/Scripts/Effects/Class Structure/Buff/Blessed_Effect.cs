public class Blessed_Effect : Multiplier_Effects
{
    /// <summary>
    /// Spiritual Defense is increased by a %
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectMultiplyAdditive"></param>
    public Blessed_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        effectStat = Stat.SpiritualDefense;
        isDebuff = false;
        hasVfx = true;
    }
}
