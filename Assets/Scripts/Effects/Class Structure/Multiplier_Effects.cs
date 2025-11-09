using UnityEngine;

public abstract class Multiplier_Effects : Effects
{
    // applies an effect multiple times over a duration
    // (i.e. "Slow: Reduce Speed by a percentage per second")
    public bool isIncremental;

    // time to wait between effect changes
    // only used for incremental effects
    public float incrementInterval;

    // makes sure incremental effect always triggers when first applied
    // only used for incremental effects
    protected bool initialApplication = true;

    protected Multiplier_Effects(float duration, float effectMultiplyAdditive) : base(duration)
    {
        effectApplication = Application.MultiplyAdditive;
        effectRate = effectMultiplyAdditive;
        totalRate = effectRate;

        // if the subclass sets incremental = true, this will be overwritten
        isIncremental = false;
        incrementInterval = 0f;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        switch (effectStat)
        {
            case Stat.HP:
                entityAttributes.hitPoints = (int)(entityAttributes.hitPoints * totalRate);
                break;
            case Stat.Speed:
                entityAttributes.speed *= totalRate;
                break;
            case Stat.Damage:
                entityAttributes.damageDealtBonus *= totalRate;
                break;
            case Stat.DamageTaken:
                entityAttributes.damageTakenBonus *= totalRate;
                break;
            case Stat.BasicDefense:
                entityAttributes.basicDefense = (int)(entityAttributes.basicDefense * totalRate);
                break;
            case Stat.SpiritualDefense:
                entityAttributes.spiritualDefense = (int)(entityAttributes.spiritualDefense * totalRate);
                break;
        }
        
        if (increment)
        {
            CompoundTotalEffect();
        }
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        switch (effectStat)
        {
            case Stat.Damage:
                // TODO incorporate weapon
                // don't return here! we also want to modify damageDealtBonus by calling the catch-all method ApplyEffect
                ApplyEffect(playerAttributes, increment);
                return;
            case Stat.StaminaRegeneration:
                playerAttributes.staminaRegeneration *= totalRate;
                break;
            case Stat.Luck:
                playerAttributes.luck *= totalRate;
                break;
            case Stat.FireRate:
                playerAttributes.attackPerSec *= totalRate;
                break;
            default:
                ApplyEffect(playerAttributes, increment);
                return;     
        }

        if (increment)
        {
            CompoundTotalEffect();
        }
    }

    // increase the total effect rate, which will be used on the next trigger
    public void CompoundTotalEffect()
    {
        totalRate *= effectRate;
        initialApplication = false;
    }
    
    public override bool IsIncremental()
    {
        return isIncremental;
    }

    public override float GetIncrementDuration()
    {
        return incrementInterval;
    }
}
