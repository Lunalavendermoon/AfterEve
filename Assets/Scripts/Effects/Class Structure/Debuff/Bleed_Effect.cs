using NUnit.Framework;
using UnityEngine;

public class Bleed_Effect : Multiplier_Effects
{

    /// <summary>
    /// HP decrease by a percentage per second
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    /// <param name="healthDecreasePercent"> lose this percent of HP each increment </param>
    /// <param name="timeBetweenIncrements"> time between each increment </param>
    public Bleed_Effect(float duration, float effectMultiplier, float timeBetweenIncrements) : base(duration, effectMultiplier)
    {
        // facts
        effectStat = Stat.HP;
        effectApplication = Application.Other;
        isDebuff = true;
        isIncremental = true;

        // numbers that we're not sure of yet/couuld be changed?
        incrementInterval = timeBetweenIncrements;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes)
    {
        if (Time.time - startTime > incrementInterval || initialApplication)
        {
            // TODO: deal damage to the player
            float bleedDmg = entityAttributes.hitPoints * effectRate;
            Debug.Log("Bleed amount: " + bleedDmg);
            startTime = Time.time;
            initialApplication = false;
        }
    }
}
