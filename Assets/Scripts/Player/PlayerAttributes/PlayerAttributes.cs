using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttributes", menuName = "Scriptable Objects/PlayerAttributes")]
public class PlayerAttributes : ScriptableObject
{
    // [Header("Attack")] commenting these out because they're messing with the XML :C

    /// <summary>
    /// Used to determine the base damage of the weapon.
    /// <br />
    /// <br /> Base Weapon Damage 100
    /// </summary>
    public int weaponDamage;

    /// <summary>
    /// Used to multiply the base damage of abilities from Tarot Cards.
    /// <br />
    /// <br /> Base Ability Multiplier 1.0
    /// </summary>
    public float abilityMultiplier;

    // [Header("Defense")]

    /// <summary>
    /// Used to determine player health
    /// <br />
    /// <br /> Base HP 1000
    /// </summary>
    public int hitPoints;

    /// <summary>
    /// Used for Basic Damage reduction from enemies 
    /// <br />
    /// <br /> Base Basic Defense 10
    /// </summary>
    public int basicDefence;

    /// <summary>
    /// Used for Spiritual Damage reduction from enemies 
    /// <br />
    /// <br /> Base Spiritual Defense 5
    /// </summary>
    public int spiritualDefense;

    /// <summary>
    /// Used to determine the flat damage reduction
    /// <br />
    /// <br /> Base Shield 0
    /// </summary>
    public int shield;

    //[Header("Movement")]

    /// <summary>
    /// Used to determine how fast the player moves
    /// <br />
    /// <br /> Base Speed 1.0
    /// </summary>
    public float speed;

    /// <summary>
    /// Used for dashing, each Dash requires 1.0 Stamina, and player receives immunity during dashing
    /// <br />
    /// <br /> Base Total Stamina 1.0
    /// </summary>
    public float totalStamina;

    /// <summary>
    /// Used to determine how fast Stamina regenerated per 5 seconds.
    /// <br />
    /// <br /> Base Stamina Regeneration 1
    /// </summary>
    public float staminaRegeneration;

    //[Header("Spiritual Vision")]

    /// <summary>
    /// Used to identify weaknesses and modify Weapon Damage to Spiritual Damage.
    /// <br />
    /// <br /> Used to determine how long player can maintain in Spiritual Vision 
    /// <br /> Each second of the Spiritual Vision uses 1.0 Total Spiritual Vision
    /// <br /> The player needs at least 2.0 Spiritual Vision to open it. 
    /// <br /> 
    /// <br /> Base Total Spiritual Vision 5
    /// </summary>
    public float totalSpiritualVision;

    /// <summary>
    /// Used to identify weaknesses and modify Weapon Damage to Spiritual Damage.
    /// <br />
    /// <br /> Used to determine how fast the Spiritual Vision regenerated per seconds.
    /// <br /> Spiritual Vision Regeneration only happens when the player is not using the Spiritual Vision.
    /// <br /> 
    /// <br /> Base Spiritual Vision Regeneration 1
    /// </summary>
    public float spiritualVisionRegeneration;

    //[Header("Other")]

    /// <summary>
    /// Used to determine loots dropped by enemies.
    /// <br />
    /// <br /> Base Luck 1.0 (caps at 10)
    /// </summary>
    public float luck;

    /// <summary>
    /// Used to determine NPC Pricing and Quests 
    /// <br />
    /// <br /> Base Trustworthiness 1
    /// </summary>
    public float trustworthiness;


    // Player Formulas

    /// <summary>
    /// Basic damage received from enemy Calculation 
    /// </summary>
    public int BasicDamage(int baseDamage)
    {
        return (int)(baseDamage * (1 - ((float)basicDefence / (basicDefence + 100))) - shield);
    }

    /// <summary>
    /// Spiritual damage received from enemy Calculation 
    /// </summary>
    public int SpiritualDamage(int baseDamage)
    {
        return (int)(baseDamage * (1 - ((float)spiritualDefense / (spiritualDefense + 100))) - shield);
    }

    /// <summary>
    /// Both Basic and Spiritual damage received from enemy Calculation 
    /// </summary>
    public int MixedDamage(int baseDamage)
    {
        return (int)(0.5 * baseDamage * (1 - ((float) basicDefence / ( basicDefence + 100)))) + (int)(0.5 * baseDamage * (1 - ((float) spiritualDefense / (spiritualDefense + 100)))) - shield;
    }


    /// <summary>
    /// Ability Multiplier Damage Calculation
    /// </summary>
    public int AbilityMultiplierDamage(int abilityBaseDamage)
    {
        return (int)(abilityBaseDamage * abilityMultiplier + 1);
    }


    /// <summary>
    /// Items that effect the player's Luck
    /// </summary>
    public enum LuckyItems
    {
        TarotCard,
        Potion,
        Shard
    }

    /// <summary>
    /// Basic version of Luck Calculation
    /// </summary>
    /// <param name="itemType"> type of the item affecting luck </param>
    /// <returns></returns>
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

    /// <summary>
    /// Luck Calculation that allows for Elite items
    /// </summary>
    /// <param name="itemType"> type of the item affecting luck </param>
    /// <param name="elite"> whether or not the elite calculation should be used </param>
    /// <returns></returns>
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

    /// <summary>
    /// Calculate the price of NPC goods based on Trustworthiness
    /// </summary>
    /// <param name="basePrice"> base price offerec by NPC </param>
    /// <returns></returns>
    public int NPCPricing(int basePrice)
    {
        return (int)(basePrice * (1 - ((float)trustworthiness / (trustworthiness + 50))));
    }
}
