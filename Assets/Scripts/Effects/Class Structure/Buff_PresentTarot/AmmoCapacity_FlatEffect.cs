using UnityEngine;

public class AmmoCapacity_FlatEffect : Flat_Effects
{
    public AmmoCapacity_FlatEffect(float duration, int effectFlat) : base(duration, effectFlat)
    {
        effectStat = Stat.AmmoCapacity;
        isDebuff = false;
        isIncremental = false;
    }
}
