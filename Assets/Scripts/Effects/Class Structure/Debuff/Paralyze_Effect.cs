using UnityEngine;

public class Paralyze_Effect : Effects
{
    PlayerController player;
    IPlayerState idle;

    /// <summary>
    /// Unable to move, dash, or attack
    /// </summary>
    /// <param name="duration"></param>
    public Paralyze_Effect(float duration) : base(duration)
    {
        effectStat = Stat.Movement;
        isDebuff = true;
        player = PlayerController.instance;
        idle = new Player_Idle();
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(PlayerAttributes _)
    {
        player.currentState = idle;
    }
}
