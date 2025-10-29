using UnityEngine;

public class Confused_Effect : Effects
{
    /// <summary>
    /// Player’s movement is reversed, Enemy attack’s direction is randomized
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    public Confused_Effect(float duration) : base(duration)
    {
        effectStat = Stat.Confused;
        isDebuff = true;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes)
    {
        entityAttributes.isConfused = true;
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes)
    {
        playerAttributes.speed *= -1;
        ApplyEffect(playerAttributes);
    }
}
