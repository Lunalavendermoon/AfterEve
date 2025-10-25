using UnityEngine;

public class Confused_Effect : Effects
{
    PlayerController player;

    /// <summary>
    /// Player’s movement is reversed
    /// </summary>
    /// <param name="attributes"> player attributes </param>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    public Confused_Effect(float duration) : base(duration)
    {
        effectStat = Stat.Reverse;
        isDebuff = true;
        player = PlayerController.instance;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(PlayerAttributes _)
    {
        player.horizontalInput *= -1;
        player.verticalInput *= -1;
    }
}
