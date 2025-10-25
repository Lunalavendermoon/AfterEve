using UnityEngine;

public class Paralyze_Effect : Effects
{
    PlayerController player;
    IPlayerState idle;

    /// <summary>
    /// Unable to move, dash, or attack
    /// </summary>
    /// <param name="modifiedAttributes"></param>
    /// <param name="duration"></param>
    public Paralyze_Effect(PlayerAttributes modifiedAttributes, float duration) : base(modifiedAttributes, duration)
    {
        effectStat = Stat.Movement;
        isDebuff = true;
        player = PlayerController.instance;
        idle = new Player_Idle();
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect()
    {
        player.currentState = idle; 
    }
}
