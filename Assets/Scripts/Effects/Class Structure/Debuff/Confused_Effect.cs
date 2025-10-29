using UnityEngine;

public class Confused_Effect : Effects
{
    /// <summary>
    /// Player’s movement is reversed
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    public Confused_Effect(float duration) : base(duration)
    {
        effectStat = Stat.Reverse;
        isDebuff = true;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        playerAttributes.speed *= -1;
        playerAttributes.isConfused = true;
    }
}
