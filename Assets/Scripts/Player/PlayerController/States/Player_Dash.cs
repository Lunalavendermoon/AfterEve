using System;
using UnityEngine;

public class Player_Dash : IPlayerState
{

    public PlayerAttributes playerAttributes;
    public float dashPower = 6f;
    public float dashDuration;
    public static float dashStartTime;
    public Vector3 dashDirection;
    public float dashCooldown;

    public static bool dashActive;

    public static event Action OnDash;
    public static event Action OnDisplaced;

    public void EnterState(PlayerController player, PlayerAttributes playerAttributes)
    {
        this.playerAttributes = playerAttributes;
        dashDuration = playerAttributes.dashDuration;
        dashCooldown = playerAttributes.dashCooldown;

        // Prevent dashing during cooldown
        Debug.Log("time since last dash: " + (Time.time - dashStartTime));
        Debug.Log("total time between dashes required: " + (dashDuration + dashCooldown));
        if(dashStartTime != 0 && Time.time - dashStartTime < dashDuration + dashCooldown) {
            Debug.Log("Failed to dash - dash in cooldown");
            return;
        }

        dashActive = true;
        OnDash?.Invoke();
        dashStartTime = Time.time;
        PlayerUtilityUI.Instance.triggerDashUsedUI();

        if (player.horizontalInput > 0 && player.verticalInput > 0)
        {
            dashDirection = new Vector3(player.horizontalInput / (float) Math.Sqrt(2), player.verticalInput / (float) Math.Sqrt(2), 0);
        }
        else
        {
            dashDirection = new Vector3(player.horizontalInput, player.verticalInput, 0);
        }   
    }

    public void CheckState(PlayerController player)
    {
        if (Time.time - dashStartTime > dashDuration) {
            dashActive = false;
            player.ChangeState(new Player_Idle());
        }
    }

    public void UpdateState(PlayerController player)
    {
        if(!dashActive) return;
        float deceleration = dashDuration - (Time.time - dashStartTime);
        deceleration *= deceleration;
        player.transform.position += 5 * player.playerAttributes.speed * dashPower * deceleration * Time.deltaTime * dashDirection;

        // allow movement as well
        Vector3 currentPosition = new Vector3();
        currentPosition.x += 5 * player.horizontalInput * player.playerAttributes.speed * Time.deltaTime;
        currentPosition.y += 5 * player.verticalInput * player.playerAttributes.speed * Time.deltaTime;

        if (player.horizontalInput > 0 && player.verticalInput > 0)
        {
            currentPosition.x *= 1 / (float)Math.Sqrt(2);
            currentPosition.y *= 1 / (float)Math.Sqrt(2);
        }

        currentPosition += player.transform.position;
        player.transform.position = currentPosition;

        OnDisplaced?.Invoke();
    }
    
    public void ExitState(PlayerController player)
    {

    }
}
