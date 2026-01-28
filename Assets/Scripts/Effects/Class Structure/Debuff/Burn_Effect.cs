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
        effectApplication = Application.Flat;
        isDebuff = true;
        isIncremental = true;

        // numbers that we're not sure of yet/couuld be changed?
        incrementInterval = timeBetweenIncrements;

        iconType = IconType.DebuffBurn;
    }

    public override bool IsIncremental()
    {
        return true;
    }

    public override float GetIncrementDuration()
    {
        return incrementInterval;
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        if (increment)
        {
            PlayerController.instance.TakeDamage((int)effectRate, DamageInstance.DamageSource.Effect, DamageInstance.DamageType.Physical);

            Debug.Log("Burn amount: " + effectRate);
            initialApplication = false;
        }
    }

    public override void ApplyEnemyEffect(EnemyAttributes enemyAttributes, EnemyBase enemy, bool increment)
    {   
        if (increment)
        {
            enemy.TakeDamage((int)effectRate, DamageInstance.DamageSource.Effect, DamageInstance.DamageType.Physical);

            Debug.Log("Burn amount: " + effectRate);
            initialApplication = false;
        }
    }
}
