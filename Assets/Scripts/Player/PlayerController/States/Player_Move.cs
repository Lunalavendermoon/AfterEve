using System;
using UnityEngine;

public class Player_Move : IPlayerState
{
    public static event Action OnDisplaced;

    public static int speedCoefficient = 5;

    private const float DashInputDeadzone = 0.01f;

    public void EnterState(PlayerController player)
    {
        player.playerAnimation.playRunAnimation();
    }

    public void CheckState(PlayerController player)
    {
        if (player.horizontalInput == 0 && player.verticalInput == 0) player.ChangeState(new Player_Idle());

        if (player.playerInput.Player.Dash.triggered)
        {
            Vector2 input = new Vector2(player.horizontalInput, player.verticalInput);
            if (input.sqrMagnitude < DashInputDeadzone * DashInputDeadzone)
            {
                // Not moving: dash does nothing and does NOT go on cooldown.
                return;
            }

            float dashDuration = player.playerAttributes.dashDuration;
            float dashCooldown = player.playerAttributes.dashCooldown;

            if (Player_Dash.dashStartTime == 0f || Time.time - Player_Dash.dashStartTime >= dashDuration + dashCooldown)
            {
                player.ChangeState(new Player_Dash());
            }
        }
    }

    public void UpdateState(PlayerController player)
    {
        PlayerMovementUI.Instance.SetMoveUIDirection(player.horizontalInput, player.verticalInput);
    }

    public void FixedUpdateState(PlayerController player)
    {
        Vector2 input = new Vector2(player.horizontalInput, player.verticalInput);
        if (input.sqrMagnitude > 1f) input = input.normalized;

        float unitsPerSecond = IPlayerState.speedCoefficient * player.playerAttributes.speed;
        Vector2 delta = input * unitsPerSecond * Time.fixedDeltaTime;

        player.TryMove(delta);

        OnDisplaced?.Invoke();
    }

    public void ExitState(PlayerController player)
    {
    }
}
