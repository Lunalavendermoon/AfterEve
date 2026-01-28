using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttributes", menuName = "Scriptable Objects/PlayerAttributes")]
public class PlayerAttributes : EntityAttributes
{
    // Player Attributes

    [Header("Player Attack")]
    public float attackPerSec;
    public int Ammo;
    public float reloadSpeed;
    public int bullets;
    public float bulletSpread;
    public bool hasBulletBounce;
    public int bulletBounces;
    public float bulletBounceDmgDecrease;
    public int bulletPierces;
    public int enemiesChained;
    public int chainRadius;
    public float chainTime;
    public float chainDmg;
    public float physicalAdditionalDmg;
    public float spiritualAdditionalDmg;
    

    [Header("Player Defense")]
    public int shield;
    public int hitCountShield;
    public float chainShieldIncrease;

    [Header("Player Movement")]
    public float totalStamina;
    public float staminaRegeneration;
    public float dashPower;
    public float dashDuration;
    public float dashCooldown;

    [Header("Spiritual Vision")]
    public float totalSpiritualVision;
    public float spiritualVisionRegeneration;
    public bool isEnlightened;

    [Header("Player Other")]
    public float luck;
    public float trustworthiness;

    [Header("Past Tarot (DON'T MANUALLY EDIT - set to false)")]
    public bool magicianPastBonusCoin;
    public bool empressPast;
    public bool hermitPast;


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

    // Ability MultiplyAdditive Damage Calculation
    public int AbilityMultiplyAdditiveDamage(float abilityBaseDamage)
    {
        return (int)(damage * (1f + damageDealtBonus));
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
