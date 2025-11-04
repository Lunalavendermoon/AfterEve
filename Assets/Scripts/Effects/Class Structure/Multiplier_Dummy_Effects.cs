// used to apply ALL additive multiplier effects for a single stat
using UnityEngine;

public class Multiplier_Dummy_Effects : Multiplier_Effects
{
    public Multiplier_Dummy_Effects(float effectMultiplyAdditive, Stat stat) : base(1f, effectMultiplyAdditive)
    {
        effectApplication = Application.MultiplyAdditive;
        effectRate = effectMultiplyAdditive;
        totalRate = effectRate;

        effectStat = stat;

        // if the subclass sets incremental = true, this will be overwritten
        isIncremental = false;
        incrementInterval = 0f;
    }
}