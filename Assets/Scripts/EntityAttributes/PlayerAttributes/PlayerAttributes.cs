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
    public int bulletBounces;
    public float bulletBounceDmgDecrease;
    public int bulletPierces;
    public float bulletPierceDmgDecrease;
    public float physicalAdditionalDmg;
    public float spiritualAdditionalDmg;

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
                damageTaken = (int)(baseDamage * (1 - ((float)basicDefense / (basicDefense + 100))));
                break;
            case EnemyAttributes.DamageType.Spiritual:
                damageTaken = (int)(baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100))));
                break;
            case EnemyAttributes.DamageType.Mixed:
                damageTaken = (int)(0.5 * baseDamage * (1 - ((float)basicDefense / (basicDefense + 100)))) + (int)(0.5 * baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100))));
                break;
        }

        return (int)(damageTaken * (1f + damageTakenBonus));
    }

    // Ability MultiplyAdditive Damage Calculation
    public int AbilityMultiplyAdditiveDamage(float abilityBaseDamage)
    {
        return (int)(damage * (abilityBaseDamage + damageDealtBonus));
    }

    // Trustworthiness Calculation
    public int NPCPricing(int basePrice)
    {
        return (int)(basePrice * (1 - (trustworthiness / (trustworthiness + 50))));
    }

    public float GetAdjustedLuck()
    {
        return luck + StaticGameManager.luckyCoins;
    }
}
