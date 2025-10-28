using UnityEngine;

public abstract class Flat_Effects : Effects
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

    // the flat amount the effect is altered
    protected Flat_Effects(float duration, int effectFlat) : base(duration)
    {
        effectRate = effectFlat;
        // set effectApplication for each child class separately
        // set it to Additive to apply the effect before multipliers, set it to Flat to apply it after multipliers
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        switch (effectStat)
        {
            case Stat.HP:
                playerAttributes.hitPoints += (int)effectRate;
                break;
            case Stat.Speed:
                playerAttributes.speed += effectRate;
                break;
            case Stat.Damage:
                // TODO incorporate weapon
                playerAttributes.damageDealtMultiplier += effectRate;
                break;
            case Stat.BasicDefense:
                playerAttributes.basicDefence += (int)effectRate;
                break;
            case Stat.SpiritualDefense:
                playerAttributes.spiritualDefense += (int)effectRate;
                break;
            case Stat.StaminaRegeneration:
                playerAttributes.staminaRegeneration += effectRate;
                break;
            case Stat.Luck:
                playerAttributes.luck += effectRate;
                break;
            case Stat.Shield:
                playerAttributes.shield += (int)effectRate;
                break;
        }
    }
}
