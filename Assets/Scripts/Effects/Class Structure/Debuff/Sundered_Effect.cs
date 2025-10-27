using NUnit.Framework;
using UnityEngine;

public class Sundered_Effect : Multiplier_Effects
{
    /// <summary>
    /// Basic Defense is reduced by a %
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    /// <param name="defenseDecreasePercent"> lose this percent of Basic Defense </param>
    public Sundered_Effect(float duration, float effectMultiplier) : base(duration, effectMultiplier)
    {
        // facts
        effectStat = Stat.BasicDefense;
        isDebuff = true;
        isIncremental = false;
    }
}
