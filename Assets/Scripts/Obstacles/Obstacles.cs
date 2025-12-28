using UnityEngine;

[CreateAssetMenu(
    fileName = "ObstacleData",
    menuName = "Scriptable Objects/Obstacles/ObstacleData"
)]
public class ObstacleData : ScriptableObject
{
    [Header("Core")]
    public ObstacleType obstacleType;

    [Header("Damage")]
    public bool dealsDamage;
    public DamageType damageType;
    public int baseDamage;
    public float damageInterval; // for DoT obstacles

    [Header("Status Effects")]
    public bool appliesFreeze;
    public float freezeDuration;

    [Header("Interaction")]
    public bool isBreakable;
    public bool chainTriggerable;

    [Header("Transport")]
    public bool isTeleport;
    public Transform teleportTarget;

    [Header("Spiritual")]
    public bool spiritualPassable;

    //enum defns

    public enum ObstacleType
    {
        Wall,
        Explosive,
        Hazard,
        Trap,
        Transport
    }

    public enum DamageType
    {
        Basic,
        Spiritual,
        Mixed
    }

    //dmg + effect calc

    public int CalculateDamageTo(EntityAttributes target)
    {
        if (!dealsDamage || target == null)
            return 0;

        float reduction = damageType switch
        {
            DamageType.Basic =>
                target.basicDefense / (target.basicDefense + 100f),

            DamageType.Spiritual =>
                target.spiritualDefense / (target.spiritualDefense + 100f),

            DamageType.Mixed =>
                0.5f * (
                    target.basicDefense / (target.basicDefense + 100f) +
                    target.spiritualDefense / (target.spiritualDefense + 100f)
                ),

            _ => 0f
        };

        float damage = 1f + baseDamage * (1f - reduction);
        return Mathf.RoundToInt(damage);
    }

    public float CalculateFreezeDuration(EntityAttributes target)
    {
        if (!appliesFreeze || target == null)
            return 0f;

        return freezeDuration * (1f - (target.resistance / (target.resistance + 50f)));
    }
}
