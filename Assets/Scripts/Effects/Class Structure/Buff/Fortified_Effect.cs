public class Fortified_Effect : Multiplier_Effects
{
    /// <summary>
    /// Basic Defense is increased by a %
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectMultiplyAdditive"></param>
    public Fortified_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        effectStat = Stat.BasicDefense;
        isDebuff = false;
    }
}
