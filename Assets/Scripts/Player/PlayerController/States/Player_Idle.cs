using System;
using UnityEngine;
using UnityEngine.XR;

public class Player_Idle : IPlayerState
{
    public static event Action OnIdleEnter;
    public static event Action OnIdleExit;
    public void EnterState(PlayerController player)
    {
        if (!PlayerController.instance.playerAttributes.isParalyzed)
        {
            // Don't invoke for idle because of paralysis
            // TODO: should we invoke even when paralyzed?
            OnIdleEnter?.Invoke();
        }

        player.playerAnimation.playIdleAnimation();
        PlayerMovementUI.Instance.SetMoveUIDirection(0f, 0f);
    }

    public void CheckState(PlayerController player)
    {
        if (player.horizontalInput != 0 || player.verticalInput != 0) player.ChangeState(new Player_Move());
    }

    public void UpdateState(PlayerController player)
    {
        
    }
    
    public void ExitState(PlayerController player)
    {
        OnIdleExit?.Invoke();
    }
}
