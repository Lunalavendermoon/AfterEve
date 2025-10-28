using UnityEngine;

public class Blindness_Effect : Effects
{
    /// <summary>
    /// Unable to move, dash, or attack
    /// </summary>
    /// <param name="duration"></param>
    public Blindness_Effect(float duration) : base(duration)
    {
        effectStat = Stat.SpiritualVision;
        isDebuff = true;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        // TODO disable spiritual vision when isBlind is true
        playerAttributes.isBlind = true;
    }
}
