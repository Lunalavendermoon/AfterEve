public class Blessed_Effect : Multiplier_Effects
{
    /// <summary>
    /// Spiritual Defense is increased by a %
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectMultiplier"></param>
    public Blessed_Effect(float duration, float effectMultiplier) : base(duration, effectMultiplier)
    {
        effectStat = Stat.SpiritualDefense;
        isDebuff = false;
    }
}
