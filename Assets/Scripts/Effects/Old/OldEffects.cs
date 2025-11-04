using UnityEngine;

[CreateAssetMenu(fileName = "Effects", menuName = "Scriptable Objects/Effects")]
public class OldEffects : ScriptableObject
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
        Reverse
    }

    // how the effect is applied
    public enum Application
    {
        // applies flat number over the duration before all other bonuses
        Additive,

        // applies a percentage over the duration
        // (i.e. "Strength: Damage of all types increase by a %")
        // applied after Additive but before Flat
        MultiplyAdditive,

        // applies flat number over the duration
        // (i.e. "Shield: Shield is increased by a flat number")
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

    // stat effected
    public Stat effectStat;

    // how the stat is applied
    public Application effectApplication;

    // permanent effect (i.e. tarot buffs)
    public bool isPermanent;

    // how long the effect lasts for
    // only used for non-permanent effects
    public float effectDuration;

    // the percentage or flat rate at which effectStat is altered
    public float effectRate;

    // applies an effect multiple times over a duration
    // (i.e. "Slow: Reduce Speed by a percentage per second")
    public bool isIncremental;

    // time to wait between effect changes
    // only used for incremental effects
    public float incrementInterval;

    public bool isDebuff;
}
