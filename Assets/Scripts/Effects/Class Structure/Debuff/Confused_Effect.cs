using UnityEngine;

public class Confused_Effect : Effects
{
    PlayerController player;

    /// <summary>
    /// Player’s movement is reversed
    /// </summary>
    /// <param name="attributes"> player attributes </param>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    public Confused_Effect(PlayerAttributes modifiedAttributes, float duration) : base(modifiedAttributes, duration)
    {
        effectStat = Stat.Reverse;
        isDebuff = true;
        player = PlayerController.instance;
    }

    public override void ApplyEffect()
    {
        player.horizontalInput *= -1;
        player.verticalInput *= -1;
    }
}
