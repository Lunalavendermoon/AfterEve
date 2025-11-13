using UnityEngine;

public class DamageInstance
{
    public enum DamageSource
    {
        Player,
        Enemy,
        Effect
    }

    public enum DamageType
    {
        Basic,
        Spiritual,
        Mixed
    }

    public DamageSource damageSource;
    public DamageType damageType;

    public int beforeReduction;
    public int afterReduction;

    public bool hitWeakPoint;

    public DamageInstance(DamageSource damageSource, DamageType damageType, int beforeReduction, int afterReduction, bool hitWeakPoint = false)
    {
        this.damageSource = damageSource;
        this.damageType = damageType;
        this.beforeReduction = beforeReduction;
        this.afterReduction = afterReduction;
        this.hitWeakPoint = hitWeakPoint;
    }
}