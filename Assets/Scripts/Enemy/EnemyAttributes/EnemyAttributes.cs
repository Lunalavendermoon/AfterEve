using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "EnemyAttributes", menuName = "Scriptable Objects/EnemyAttributes")]
public class EnemyAttributes : ScriptableObject
{
    [Header("Attack")]
    public int enemyDamage;

    public enum DamageType
    {
        Basic,
        Spiritual,
        Mixed
    }
    public DamageType damageType;
    public int attackRadius;
    public float attackRate;

    [Header("Defense")]

    public int hitPoints;
    public int weight;

    public int basicDefense;
    public int spiritualDefense;

    [Header("Movement")]
    public float speed;

    [Header("Status Effects")]
    public float resistance;

    [Header("Detection")]
    public float detection_radius;

    // Enemy Formulas

    // Damage received from player Calculation
    public int DamageCalculation(int baseDamage, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Basic:
                return (int)(1 + baseDamage * (1 - ((float)basicDefense / (basicDefense + 100))));
            case DamageType.Spiritual:
                return (int)(1 + baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100))));
            case DamageType.Mixed:
                return (int)(1 + 0.5f * baseDamage * (1 - ((float)basicDefense / (basicDefense + 100))) + 0.5f * baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100))));
        }

        return 0;
    }

    public int DamageCalculation(int baseDamage)
    {
        return (int)(1 + baseDamage * (1 - ((float)basicDefense / (basicDefense + 100))));
    }

    public float ResistanceCalculation(float effectDuration)
    {
        return effectDuration * (1 - (resistance / (resistance + 50)));
    }
}
