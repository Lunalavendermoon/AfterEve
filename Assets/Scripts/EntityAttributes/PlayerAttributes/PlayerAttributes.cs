using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttributes", menuName = "Scriptable Objects/PlayerAttributes")]
public class PlayerAttributes : EntityAttributes
{
    // Player Attributes

    [Header("Player Attack")]
    public float abilityMultiplier;
    public float attackPerSec;
    public int Ammo;
    public float reloadSpeed;
    public int bullets;
    public int bulletSpread;
    public int angle;

    [Header("Player Defense")]
    public int shield;

    [Header("Player Movement")]
    public float totalStamina;
    public float staminaRegeneration;

    [Header("Spiritual Vision")]
    public float totalSpiritualVision;
    public float spiritualVisionRegeneration;
    public bool isBlind;
    public bool isEnlightened;

    [Header("Player Other")]
    public float luck;
    public float trustworthiness;
    public bool hasKnockback;


    // Player Formulas

    // Damange received from enemy Calculation
    public int DamageCalculation(int baseDamage, EnemyAttributes.DamageType damageType)
    {
        int damageTaken = 0;

        switch (damageType)
        {
            case EnemyAttributes.DamageType.Basic:
                damageTaken = (int)(baseDamage * (1 - ((float)basicDefense / (basicDefense + 100))) - shield);
                break;
            case EnemyAttributes.DamageType.Spiritual:
                damageTaken = (int)(baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100))) - shield);
                break;
            case EnemyAttributes.DamageType.Mixed:
                damageTaken = (int)(0.5 * baseDamage * (1 - ((float)basicDefense / (basicDefense + 100)))) + (int)(0.5 * baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100)))) - shield;
                break;
        }

        return (int)(damageTaken * (1f + damageTakenBonus));
    }

    // Ability Multiplier Damage Calculation
    public int AbilityMultiplierDamage(int abilityBaseDamage)
    {
        return (int)(abilityBaseDamage * abilityMultiplier * (1f + damageDealtBonus) + 1);
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
