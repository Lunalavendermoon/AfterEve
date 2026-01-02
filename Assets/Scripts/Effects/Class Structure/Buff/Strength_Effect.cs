public class Strength_Effect : Multiplier_Effects
{
    /// <summary>
    /// Damage of all types increase by a %
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectMultiplyAdditive"></param>
    public Strength_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        effectStat = Stat.Damage;
        isDebuff = false;

        iconType = IconType.BuffStrength;
        hasVfx = true;
    }
}
