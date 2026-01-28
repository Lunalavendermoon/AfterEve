using UnityEngine;

public abstract class Shield
{
    // order of enum determines priority of shield depletion!
    public enum ShieldType
    {
        HitCount,
        Regular
    }

    public ShieldType shieldType;

    public int shieldAmount;

    // permanent shield
    public bool isPermanent = false;

    // duration of shield. only used for non-permanent shield
    public float shieldDuration;

    public Shield(int amount, ShieldType type, float duration)
    {
        shieldAmount = amount;
        shieldType = type;

        shieldDuration = duration;

        if (duration <= 0) isPermanent = true;
    }
    
    public abstract int TakeShieldDamage(int amount);

    //function to update vfx based on time, only used by luck and enlighten currently
    public virtual void UpdateVFXBasedOnTime(float time_remaining, PlayerVFXManager vfx)
    {
        return;
    }

    public bool IsDepleted()
    {
        return shieldAmount <= 0;
    }

    public void GainShield(int amount)
    {
        shieldAmount += amount;
    }
}
