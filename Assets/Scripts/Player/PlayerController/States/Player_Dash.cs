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

    public void EnterState(PlayerController player)
    {
        Vector2 input = new Vector2(player.horizontalInput, player.verticalInput);

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

        dashActive = true;
        OnDash?.Invoke();
        dashStartTime = Time.time;
        PlayerMovementUI.Instance.triggerDashUsedUI();
    }

    public void CheckState(PlayerController player)
    {
        if (Time.time - dashStartTime > dashDuration)
        {
            dashActive = false;
            player.ChangeState(new Player_Idle());
        }
    }

    public void UpdateState(PlayerController player)
    {
    }

    public void FixedUpdateState(PlayerController player)
    {
        if (!dashActive) return;

        // Interpret dashPower as total dash distance (world units).
        // This makes dash distance easy to tune and prevents "way too far" dashes.
        float dashUnitsPerSecond = dashPower / Mathf.Max(0.001f, dashDuration);

        Vector2 dashDelta = (Vector2)dashDirection * dashUnitsPerSecond * Time.fixedDeltaTime;
        player.TryMove(dashDelta);

        OnDisplaced?.Invoke();
    }

    public void ExitState(PlayerController player)
    {
    }
}
