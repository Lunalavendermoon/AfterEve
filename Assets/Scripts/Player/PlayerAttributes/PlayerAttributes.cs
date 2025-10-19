using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttributes", menuName = "Scriptable Objects/PlayerAttributes")]
public class PlayerAttributes : ScriptableObject
{
    // Player Attributes

    [Header("Attack")]
    public int weaponDamage;
    public float abilityMultiplier;

    [Header("Defense")]
    public int hitPoints;
    public int basicDefence;
    public int spiritualDefense;
    public int shield;

    [Header("Movement")]
    public float speed;
    public float totalStamina;
    public float staminaRegeneration;

    [Header("Spiritual Vision")]
    public float totalSpiritualVision;
    public float spiritualVisionRegeneration;

    [Header("Other")]
    public float luck;
    public float trustworthiness;


    // Player Formulas

    // Damange received from enemy Calculation
    public int DamageCalculation(int baseDamage, EnemyAttributes.DamageType damageType)
    {
        switch (damageType)
        {
            case EnemyAttributes.DamageType.Basic:
                return (int)(baseDamage * (1 - ((float)basicDefence / (basicDefence + 100))) - shield);
            case EnemyAttributes.DamageType.Spiritual:
                return (int)(baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100))) - shield);
            case EnemyAttributes.DamageType.Mixed:
                return (int)(0.5 * baseDamage * (1 - ((float)basicDefence / (basicDefence + 100)))) + (int)(0.5 * baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100)))) - shield;
        }

        return 0;
    }

    // Ability Multiplier Damage Calculation
    public int AbilityMultiplierDamage(int abilityBaseDamage)
    {
        return (int)(abilityBaseDamage * abilityMultiplier + 1);
    }


    // Luck Calculation
    public enum LuckyItems
    {
        TarotCard,
        Potion,
        Shard
    }

    // Basic Version of Luck Calculation
    public float LuckCalculation(LuckyItems itemType)
    {
        switch (itemType)
        {
            case LuckyItems.TarotCard:
                return 0.01f * luck;
            case LuckyItems.Potion:
                return 0.05f * luck;
            case LuckyItems.Shard:
                return Random.Range(0, 2) * luck;
        }
        return 0;
    }

    // Elite Version of Luck Calculation
    public float LuckCalculation(LuckyItems itemType, bool elite)
    {
        if (!elite) return LuckCalculation(itemType);

        switch (itemType)
        {
            case LuckyItems.TarotCard:
                return 0.05f * luck;
            case LuckyItems.Potion:
                return 0.10f * luck;
            case LuckyItems.Shard:
                return Random.Range(5, 10) * luck;
        }

        return 0;
    }

    // Trustworthiness Calculation
    public int NPCPricing(int basePrice)
    {
        return (int)(basePrice * (1 - ((float)trustworthiness / (trustworthiness + 50))));
    }
}
