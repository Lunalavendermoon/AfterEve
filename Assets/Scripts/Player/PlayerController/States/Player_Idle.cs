using System;
using UnityEngine;

public class Player_Idle : IPlayerState
{
    public static event Action OnIdleEnter;
    public static event Action OnIdleExit;

    public void EnterState(PlayerController player)
    {
        if (!PlayerController.instance.playerAttributes.isParalyzed)
        {
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

    public void FixedUpdateState(PlayerController player)
    {
        // Intentionally no movement while idle.
        // Keep empty to avoid throwing in PlayerController.FixedUpdate.
    }

    public void ExitState(PlayerController player)
    {
        OnIdleExit?.Invoke();
    }
}
