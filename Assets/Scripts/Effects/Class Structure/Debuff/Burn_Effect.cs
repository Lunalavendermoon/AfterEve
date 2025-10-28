using NUnit.Framework;
using UnityEngine;

public class Burn_Effect : Flat_Effects
{
    /// <summary>
    /// HP decrease by a flat number per second
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    /// <param name="healthDecreasePercent"> lose this amount of HP each increment </param>
    /// <param name="timeBetweenIncrements"> time between each increment </param>
    public Burn_Effect(float duration, int effectFlat, float timeBetweenIncrements) : base(duration, effectFlat)
    {
        // facts
        effectStat = Stat.HP;
        effectApplication = Application.Other;
        isDebuff = true;
        isIncremental = true;

        // numbers that we're not sure of yet/couuld be changed?
        incrementInterval = timeBetweenIncrements;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        if (Time.time - startTime > incrementInterval || initialApplication)
        {
            // TODO: deal damage to the player
            Debug.Log("Burn amount: " + effectRate);
            startTime = Time.time;
            initialApplication = false;
        }
    }
}
