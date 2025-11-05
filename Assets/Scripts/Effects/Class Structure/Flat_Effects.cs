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
        totalRate = effectRate;
        // set effectApplication for each child class separately
        // set it to Additive to apply the effect before multipliers, set it to Flat to apply it after multipliers
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        switch (effectStat)
        {
            case Stat.HP:
                entityAttributes.hitPoints += (int)effectRate;
                break;
            case Stat.Speed:
                entityAttributes.speed += effectRate;
                break;
            case Stat.Damage:
                entityAttributes.damageDealtBonus += effectRate;
                break;
            case Stat.BasicDefense:
                entityAttributes.basicDefense += (int)effectRate;
                break;
            case Stat.SpiritualDefense:
                entityAttributes.spiritualDefense += (int)effectRate;
                break;
        }
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        switch (effectStat)
        {
            case Stat.Damage:
                // TODO incorporate weapon
                // don't return here! we also want to modify damageDealtBonus by calling the catch-all method ApplyEffect
                break;
            case Stat.StaminaRegeneration:
                playerAttributes.staminaRegeneration += effectRate;
                return;
            case Stat.Luck:
                playerAttributes.luck += effectRate;
                return;
        }
        // if effectStat wasn't one of the above (or it was Stat.Damage), this method gets called
        ApplyEffect(playerAttributes, increment);
    }
}
