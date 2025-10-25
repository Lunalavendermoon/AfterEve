using UnityEngine;

public class Regeneration_Effect : Multiplier_Effects
{
    /// <summary>
    /// HP regenerate by a percentage per second
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="duration"></param>
    /// <param name="effectMultiplier"></param>
    public Regeneration_Effect(float duration, float effectMultiplier) : base(duration, effectMultiplier)
    {
        effectStat = Stat.HP;
        isDebuff = false;
        isIncremental = true;
        incrementInterval = 1;
        effectApplication = Application.Flat;
    }
}
