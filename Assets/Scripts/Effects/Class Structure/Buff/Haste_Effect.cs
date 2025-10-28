using UnityEditor.Rendering;

public class Haste_Effect : Effects
{
    float speedRate;
    float stamRate;

    /// <summary>
    /// Increase Speed and Stamina Regeneration
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

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        playerAttributes.speed *= speedRate;
        playerAttributes.staminaRegeneration *= stamRate;
    }
}
