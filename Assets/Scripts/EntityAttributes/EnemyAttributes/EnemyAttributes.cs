using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttributes", menuName = "Scriptable Objects/EnemyAttributes")]
public class EnemyAttributes : EntityAttributes
{
    [Header("Enemy Attack")]
    public DamageType damageType;
    public int attackRadius;
    public float attackRate;

    [Header("Enemy Defense")]
    public int weight;

    [Header("Detection")]
    public float detection_radius;


    public enum DamageType
    {
        Basic,
        Spiritual,
        Mixed
    }

    // Enemy Formulas

    // Damage received from player Calculation
    public int DamageCalculation(int baseDamage, DamageType damageType)
    {
        int damageTaken = 0;

        switch (damageType)
        {
            case DamageType.Basic:
                damageTaken = (int)(1 + baseDamage * (1 - ((float)basicDefense / (basicDefense + 100))));
                break;
            case DamageType.Spiritual:
                damageTaken = (int)(1 + baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100))));
                break;
            case DamageType.Mixed:
                damageTaken = (int)(1 + 0.5f * baseDamage * (1 - ((float)basicDefense / (basicDefense + 100))) + 0.5f * baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100))));
                break;
        }

        return (int)(damageTaken * (1f + damageTakenBonus));
    }

    public int DamageCalculation(int baseDamage)
    {
        return (int)((1 + baseDamage * (1 - ((float)basicDefense / (basicDefense + 100)))) * (1f + damageDealtBonus));
    }

    public float ResistanceCalculation(float effectDuration)
    {
        return effectDuration * (1 - (resistance / (resistance + 50)));
    }
}
