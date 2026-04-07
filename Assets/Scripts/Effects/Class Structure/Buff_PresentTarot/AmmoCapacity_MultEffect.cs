using UnityEngine;

public class AmmoCapacity_MultEffect : Multiplier_Effects
{
    public AmmoCapacity_MultEffect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        effectStat = Stat.AmmoCapacity;
        isDebuff = false;
        isIncremental = false;
    }
}
