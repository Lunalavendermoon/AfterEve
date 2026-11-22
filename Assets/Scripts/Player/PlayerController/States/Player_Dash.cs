using System;
using UnityEngine;

public class Player_Dash : IPlayerState
{
    public float dashPower;
    public float dashDuration;
    public static float dashStartTime;
    public Vector3 dashDirection;
    public float dashCooldown;

    public static bool dashActive;

    public static event Action OnDash;
    public static event Action OnDisplaced;

    private const float DashInputDeadzone = 0.01f;
    private float dashElapsed;

    public void EnterState(PlayerController player)
    {
        Vector2 input = new Vector2(player.horizontalInput, player.verticalInput);

        if (player.playerAttributes != null && player.playerAttributes.isConfused)
        {
            input *= -1f;
        }

        // If not moving, dash does nothing and must not start cooldown.
        if (input.sqrMagnitude < DashInputDeadzone * DashInputDeadzone)
        {
            dashActive = false;
            player.ChangeState(new Player_Move());
            return;
        }

        dashPower = PlayerController.instance.playerAttributes.dashPower;
        dashDuration = PlayerController.instance.playerAttributes.dashDuration;
        dashCooldown = PlayerController.instance.playerAttributes.dashCooldown;

        if (dashStartTime != 0 && Time.time - dashStartTime < dashDuration + dashCooldown)
        {
            // Back to movement (you were already moving when dash was attempted)
            player.ChangeState(new Player_Move());
            return;
        }

        if (input.sqrMagnitude > 1f) input = input.normalized;
        dashDirection = new Vector3(input.x, input.y, 0f);

        dashElapsed = 0f;
        dashActive = true;
        player.playerAnimation?.PlayDashImpact(dashDuration);
        OnDash?.Invoke();
        dashStartTime = Time.time;
        PlayerMovementUI.Instance.triggerDashUsedUI();
        PlayerController.instance.chariotPulse(dashDuration);
    }

    public void CheckState(PlayerController player)
    {
        if (dashElapsed >= dashDuration)
        {
            if (new Vector2(player.horizontalInput, player.verticalInput).sqrMagnitude
                >= DashInputDeadzone * DashInputDeadzone)
            {
                player.ChangeState(new Player_Move());
            }
            else
            {
                player.ChangeState(new Player_Idle());
            }
        }
    }

    public void UpdateState(PlayerController player)
    {
    }

    public void FixedUpdateState(PlayerController player)
    {
        if (!dashActive || dashElapsed >= dashDuration) return;

        float previousProgress = dashElapsed / dashDuration;
        dashElapsed = Mathf.Min(dashElapsed + Time.fixedDeltaTime, dashDuration);
        float progress = dashElapsed / dashDuration;

        // Blend linear motion with easing to keep the dash responsive at both ends.
        float previousDistance = Mathf.Lerp(previousProgress, Mathf.SmoothStep(0f, 1f, previousProgress), 0.6f);
        float distance = Mathf.Lerp(progress, Mathf.SmoothStep(0f, 1f, progress), 0.6f);
        Vector2 dashDelta = (Vector2)dashDirection * (dashPower * (distance - previousDistance));
        player.TryMove(dashDelta, passThroughEnemies: true);

        OnDisplaced?.Invoke();
    }

    public void ExitState(PlayerController player)
    {
        dashActive = false;
    }
}
