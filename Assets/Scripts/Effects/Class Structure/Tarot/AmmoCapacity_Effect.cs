using UnityEngine;

public class AmmoCapacity_Effect : Multiplier_Effects
{
    public AmmoCapacity_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        effectStat = Stat.AmmoCapacity;
        isDebuff = false;
        isIncremental = false;
    }
}
