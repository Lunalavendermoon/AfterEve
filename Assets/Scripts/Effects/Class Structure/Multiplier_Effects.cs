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

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        if (isIncremental && (Time.time - startTime > incrementInterval || initialApplication))
        {
            totalRate *= effectRate;
            startTime = Time.time;
            initialApplication = false;
        }

        switch (effectStat)
        {
            case Stat.HP:
                playerAttributes.hitPoints = (int)(playerAttributes.hitPoints * totalRate);
                break;
            case Stat.Speed:
                playerAttributes.speed *= totalRate;
                break;
            case Stat.Damage:
                // TODO incorporate weapon
                break;
            case Stat.BasicDefense:
                playerAttributes.basicDefence = (int)(playerAttributes.basicDefence * totalRate);
                break;
            case Stat.SpiritualDefense:
                playerAttributes.spiritualDefense = (int)(playerAttributes.spiritualDefense * totalRate);
                break;
            case Stat.StaminaRegeneration:
                playerAttributes.staminaRegeneration *= totalRate;
                break;
            case Stat.Luck:
                playerAttributes.luck *= totalRate;
                break;
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
