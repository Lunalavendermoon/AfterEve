public class Strength_Effect : Multiplier_Effects
{
    /// <summary>
    /// Damage of all types increase by a %
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectMultiplier"></param>
    public Strength_Effect(float duration, float effectMultiplier) : base(duration, effectMultiplier)
    {
        effectStat = Stat.Damage;
        isDebuff = false;
    }
}
