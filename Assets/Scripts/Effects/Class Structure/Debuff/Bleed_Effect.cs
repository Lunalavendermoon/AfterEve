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
    public Bleed_Effect(float duration, float effectMultiplyAdditive, float timeBetweenIncrements) : base(duration, effectMultiplyAdditive)
    {
        // facts
        effectStat = Stat.HP;
        effectApplication = Application.Other;
        isDebuff = true;
        isIncremental = true;

        // numbers that we're not sure of yet/couuld be changed?
        incrementInterval = timeBetweenIncrements;

        effectApplication = Application.Multiplier;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        if (increment)
        {
            int bleedDmg = (int)(playerAttributes.maxHitPoints * effectRate);
            PlayerController.instance.TakeDamage(bleedDmg, DamageInstance.DamageSource.Effect, DamageInstance.DamageType.Physical);

            Debug.Log("Bleed amount: " + bleedDmg);
            initialApplication = false;
        }
    }

    public override void ApplyEnemyEffect(EnemyAttributes enemyAttributes, EnemyBase enemy, bool increment)
    {   
        if (increment)
        {
            int bleedDmg = (int)(enemyAttributes.maxHitPoints * effectRate);
            enemy.TakeDamage(bleedDmg, DamageInstance.DamageSource.Effect, DamageInstance.DamageType.Physical);

            Debug.Log("Bleed amount: " + bleedDmg);
            initialApplication = false;
        }
    }
}
