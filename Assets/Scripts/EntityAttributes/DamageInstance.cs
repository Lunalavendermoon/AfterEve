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

    public DamageInstance(DamageSource damageSource, DamageType damageType)
    {
        this.damageSource = damageSource;
        this.damageType = damageType;
    }
}