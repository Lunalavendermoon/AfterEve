using UnityEngine;

public abstract class Flat_Effects : Effects
{
    // the flat amount the effect is altered
    protected Flat_Effects(float duration, int effectFlat) : base(duration)
    {
        effectRate = effectFlat;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        switch (effectStat)
        {
            case Stat.HP:
                playerAttributes.hitPoints += (int)effectRate;
                break;
            case Stat.Speed:
                playerAttributes.speed += effectRate;
                break;
            case Stat.Damage:
                // TODO incorporate weapon
                break;
            case Stat.BasicDefense:
                playerAttributes.basicDefence += (int)effectRate;
                break;
            case Stat.SpiritualDefense:
                playerAttributes.spiritualDefense += (int)effectRate;
                break;
            case Stat.StaminaRegeneration:
                playerAttributes.staminaRegeneration += effectRate;
                break;
            case Stat.Luck:
                playerAttributes.luck += effectRate;
                break;
            case Stat.Shield:
                playerAttributes.shield += (int)effectRate;
                break;
        }
    }
}
