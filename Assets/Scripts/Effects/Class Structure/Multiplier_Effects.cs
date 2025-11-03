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

    protected float startTime;

    private float totalRate;

    protected Multiplier_Effects(float duration, float effectMultiplier) : base(duration)
    {
        effectApplication = Application.Multiplier;
        startTime = Time.time;
        effectRate = effectMultiplier;
        totalRate = effectRate;

        // if the subclass sets incremental = true, this will be overwritten
        isIncremental = false;
        incrementInterval = 0f;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes)
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
        
        CompoundTotalEffect();
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes)
    {
        switch (effectStat)
        {
            case Stat.Damage:
                // TODO incorporate weapon
                // don't return here! we also want to modify damageDealtBonus by calling the catch-all method ApplyEffect
                ApplyEffect(playerAttributes);
                return;
            case Stat.StaminaRegeneration:
                playerAttributes.staminaRegeneration *= totalRate;
                break;
            case Stat.Luck:
                playerAttributes.luck *= totalRate;
                break;
            default:
                ApplyEffect(playerAttributes);
                return;
        }

        CompoundTotalEffect();
    }

    // increase the total effect rate, which will be used on the next trigger
    protected void CompoundTotalEffect()
    {
        if (isIncremental && (Time.time - startTime > incrementInterval || initialApplication))
        {
            totalRate *= effectRate;
            startTime = Time.time;
            initialApplication = false;
        }
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
