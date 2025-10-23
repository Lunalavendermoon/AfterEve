using UnityEngine;

public abstract class Multiplier_Effects : Effects
{
    // applies an effect multiple times over a duration
    // (i.e. "Slow: Reduce Speed by a percentage per second")
    public bool isIncremental;

    // time to wait between effect changes
    // only used for incremental effects
    public float incrementInterval;
    private float startTime;

    protected Multiplier_Effects(PlayerAttributes attributes, float duration, float effectMultiplier) : base(attributes, duration)
    {
        effectApplication = Application.Multiplier;
        startTime = Time.time;
        effectRate = effectMultiplier;
    }

    public override void ApplyEffect()
    {
        if (isIncremental && Time.time - startTime > incrementInterval)
        {
            effectRate *= effectRate;
            startTime = Time.time;
        }

        switch (effectStat)
        {
            case Stat.HP:
                playerAttributes.hitPoints = (int)(playerAttributes.hitPoints * effectRate);
                break;
            case Stat.Speed:
                playerAttributes.speed *= effectRate;
                break;
            case Stat.Damage:
                // TODO incorporate weapon
                break;
            case Stat.BasicDefense:
                playerAttributes.basicDefence = (int)(playerAttributes.basicDefence * effectRate);
                break;
            case Stat.SpiritualDefense:
                playerAttributes.spiritualDefense = (int)(playerAttributes.spiritualDefense * effectRate);
                break;
            case Stat.StaminaRegeneration:
                playerAttributes.staminaRegeneration *= effectRate;
                break;
            case Stat.Luck:
                playerAttributes.luck *= effectRate;
                break;
        }
    }
}
