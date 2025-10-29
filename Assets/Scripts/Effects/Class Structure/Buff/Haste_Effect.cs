using UnityEditor.Rendering;

public class Haste_Effect : Effects
{
    float speedRate;
    float stamRate;

    /// <summary>
    /// Increase Speed and Stamina Regeneration - player variant with stamina regen
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="speedMultiplier"></param>
    /// <param name="staminaMultiplier"></param>
    public Haste_Effect(float duration, float speedMultiplier, float staminaMultiplier) : base(duration)
    {
        effectStat = Stat.Haste;
        isDebuff = false;

        speedRate = speedMultiplier;
        stamRate = staminaMultiplier;
    }

    /// <summary>
    /// Increase Speed - enemy variant with no stamina regen
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="speedMultiplier"></param>
    public Haste_Effect(float duration, float speedMultiplier) : base(duration)
    {
        effectStat = Stat.Haste;
        isDebuff = false;

        speedRate = speedMultiplier;
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
