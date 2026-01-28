using System.Net.NetworkInformation;
using UnityEngine;

public class DamageInstance
{
    public enum DamageSource
    {
        Player,
        Enemy,
        Effect,
        Environment
    }

    public enum DamageType
    {
        Physical,
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

    public static EnemyAttributes.DamageType ToEnemyDamageType(DamageType type)
    {
        switch (type)
        {
            case DamageType.Physical:
                return EnemyAttributes.DamageType.Basic;
            case DamageType.Spiritual:
                return EnemyAttributes.DamageType.Spiritual;
            default:
                return EnemyAttributes.DamageType.Mixed;
        }
    }
}