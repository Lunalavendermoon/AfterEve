using UnityEngine;

[CreateAssetMenu(fileName = "EffectScriptableObject", menuName = "Scriptable Objects/EffectScriptableObject")]
public class EffectScriptableObject : ScriptableObject
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
        // applies an effect multiple times over a duration
        // (i.e. "Slow: Reduce Speed by a percentage per second")
        Incremental,

        // applies a percentage over the duration
        // (i.e. "Strength: Damage of all types increase by a %")
        Multiplier,

        // applies flat number over the duration
        // (i.e. "Shield: Shield is increased by a flat number")
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

    // how long the effect lasts for
    public float effectDuration;

    // the percentage or flat rate at which effectStat is altered
    public float effectRate;
}
