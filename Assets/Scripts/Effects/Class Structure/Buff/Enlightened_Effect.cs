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
        hasVfx = true;
        iconType = IconType.BuffEnlighten;
    }

    public override void ApplyEffect(EntityAttributes _e, bool _)
    {
        // player-exclusive effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool _)
    {
        playerAttributes.isEnlightened = true;
    }

    public override void UpdateVFXBasedOnTime(float time_remaining, PlayerVFXManager vfx)
    {
        vfx.SetEnlightenTime(time_remaining);
    }
}
