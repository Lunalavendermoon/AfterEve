using UnityEngine;

public abstract class Effects
{
    // the stat affected by the effect
    public enum Stat
    {
        HP,
        Speed,
        Damage,
        BasicDefense,
        SpiritualDefense,
        SpiritualVision,
        StaminaRegeneration,
        Luck,
        Shield,
        Knockback,
        Reverse,
        Movement
    }

    // how the effect is applied
    public enum Application
    {
        // Note: please don't reorder Additive, Multiplier, and Float!
        // this is the other of operations used to apply all the effects onto the entity

        // applies flat number over the duration before all other bonuses
        Additive,

        // applies a percentage over the duration
        // (i.e. "Strength: Damage of all types increase by a %")
        // applied after Additive but before Flat
        Multiplier,

        // applies flat number over the duration
        // (i.e. "Shield: Shield is increased by a flat number")
        // applied after Additive and Multiplier
        Flat,

        // completely disable/ignore an ability/stat 
        // (i.e. "Blindness: Unable to use Spiritual Vision")
        Disable,

        // stuff that doesn't fit into the other categories
        // (i.e. Knockback, Confused, probably more in the future)
        // or maybe just make more Application types idk
        Other
    }

    // stat effected
    public Stat effectStat;

    // how the stat is applied
    public Application effectApplication;

    // the amount of flat or multiplier applied
    public float effectRate;

    // permanent effect (i.e. tarot buffs)
    public bool isPermanent = false;

    // how long the effect lasts for
    // only used for non-permanent effects
    public float effectDuration;

    // true = debuff, false = buff
    public bool isDebuff;

    public Effects(float duration)
    {
        effectDuration = duration;

        if (duration < 0) isPermanent = true;
    }

    // should be called every frame by the EffectsManager when in effect
    public abstract void ApplyEffect(PlayerAttributes playerAttributes);

    public virtual bool IsIncremental()
    {
        return false;
    }

    public virtual float GetIncrementDuration()
    {
        return 0f;
    }
}
