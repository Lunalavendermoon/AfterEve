using UnityEngine;

public class EntityAttributes : ScriptableObject
{
    [Header("Attack")]
    public int damage;
    public float damageDealtBonus;
    public bool ignoreBasicDef;
    public bool ignoreSpiritualDef;
    
    [Header("Defense")]
    public int hitPoints;
    public int basicDefense;
    public int spiritualDefense;
    public float damageTakenBonus;

    [Header("Movement")]
    public float speed;
    public bool isParalyzed;
    public bool isConfused;

    [Header("Other")]
    public float resistance;
    public bool isBlind;
    public bool hasKnockback;
}