using UnityEngine;

public class Enlightened_Effect : Effects
{
    /// <summary>
    /// Spiritual Vision may be used at no cost for a specific time
    /// </summary>
    /// <param name="duration"></param>
    public Enlightened_Effect(float duration) : base(duration)
    {
        effectStat = Stat.SpiritualVision;
        isDebuff = false;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        playerAttributes.isEnlightened = true;
    }
}
