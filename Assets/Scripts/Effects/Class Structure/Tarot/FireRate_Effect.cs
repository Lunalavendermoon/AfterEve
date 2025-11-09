using UnityEngine;

public class FireRate_Effect : Multiplier_Effects
{
    public FireRate_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        effectStat = Stat.FireRate;
        isDebuff = false;
        isIncremental = false;
    }
}
