using UnityEngine;

public abstract class Effects
{
    // the stat affected by the effect
    public enum Stat
    {
        HP,
        Speed,
        Damage,
        DamageTaken,
        BasicDefense,
        SpiritualDefense,
        SpiritualVision,
        StaminaRegeneration,
        Luck,
        Knockback,
        Confused,
        Movement,
        Haste,
        FireRate,
        BulletBounce,
        AmmoCapacity,
        HitCountShield
    }

    // how the effect is applied
    public enum Application
    {
        // Note: please don't reorder the effect applications!
        // this is the other of operations used to apply all the effects onto the entity

        // applies flat number over the duration before all other bonuses
        Additive,

        // applies a percentage over the duration -- all percentages are SUMMED UP
        // (i.e. "Strength: Damage of all types increase by a %")
        // applied after Additive but before Flat
        MultiplyAdditive,

        // applies a percentage over the duration
        // (i.e. "Strength: Damage of all types increase by a %")
        // applied after Additive but before Flat
        Multiplier,

        // applies flat number over the duration
        // applied after Additive and MultiplyAdditive
        Flat,

        // completely disable/ignore an ability/stat 
        // (i.e. "Blindness: Unable to use Spiritual Vision")
        Disable,

        // stuff that doesn't fit into the other categories
        // (i.e. Knockback, Confused, probably more in the future)
        // or maybe just make more Application types idk
        Other
    }

    public enum IconType
    {
        None,
        BuffDefense,
        BuffSpeed,
        BuffRegen,
        BuffStrength
    }

    // stat effected
    public Stat effectStat;

    // how the stat is applied
    public Application effectApplication;

    // the amount of flat or multiplier applied
    public float effectRate;
    public float totalRate;

    // permanent effect (i.e. tarot buffs)
    public bool isPermanent = false;

    // how long the effect lasts for
    // only used for non-permanent effects
    public float effectDuration;

    // true = debuff, false = buff
    public bool isDebuff;

    //True if has vfx, false ow
    public bool hasVfx;

    public IconType iconType = IconType.None;

    public Effects(float duration)
    {
        effectDuration = duration;

        if (duration <= 0) isPermanent = true;
    }

    // should be called every frame by the EffectsManager when in effect
    public abstract void ApplyEffect(EntityAttributes entityAttributes, bool increment);

    // by default, an effect does the same thing when applied to a player or an enemy
    // override these to change effect behavior!

    public virtual void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        ApplyEffect(playerAttributes, increment);
    }

    // since enemy isn't a singleton, we need to pass in the specific enemy entity in order to interact with its controller
    // not sure if the enemy field will be used, but including it here as an optional argument just in case we do
    public virtual void ApplyEnemyEffect(EnemyAttributes enemyAttributes, EnemyBase enemy, bool increment)
    {
        ApplyEffect(enemyAttributes, increment);
    }

    public virtual bool IsIncremental()
    {
        return false;
    }

    public virtual float GetIncrementDuration()
    {
        return 0f;
    }
}
