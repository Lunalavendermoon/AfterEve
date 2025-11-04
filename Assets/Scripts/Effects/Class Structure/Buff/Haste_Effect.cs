using UnityEditor.Rendering;

public class Haste_Effect : Effects
{
    float speedRate;
    float stamRate;

    /// <summary>
    /// Increase Speed and Stamina Regeneration - player variant with stamina regen
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="speedMultiplyAdditive"></param>
    /// <param name="staminaMultiplyAdditive"></param>
    public Haste_Effect(float duration, float speedMultiplyAdditive, float staminaMultiplyAdditive) : base(duration)
    {
        effectStat = Stat.Haste;
        isDebuff = false;

        speedRate = speedMultiplyAdditive;
        stamRate = staminaMultiplyAdditive;
    }

    /// <summary>
    /// Increase Speed - enemy variant with no stamina regen
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="speedMultiplyAdditive"></param>
    public Haste_Effect(float duration, float speedMultiplyAdditive) : base(duration)
    {
        effectStat = Stat.Haste;
        isDebuff = false;

        speedRate = speedMultiplyAdditive;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes)
    {
        entityAttributes.speed *= speedRate;
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes)
    {
        playerAttributes.staminaRegeneration *= stamRate;
        ApplyEffect(playerAttributes);
    }
}
