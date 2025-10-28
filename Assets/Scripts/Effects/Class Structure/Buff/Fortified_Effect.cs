public class Fortified_Effect : Multiplier_Effects
{
    /// <summary>
    /// Basic Defense is increased by a %
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectMultiplier"></param>
    public Fortified_Effect(float duration, float effectMultiplier) : base(duration, effectMultiplier)
    {
        effectStat = Stat.BasicDefense;
        isDebuff = false;
    }
}
