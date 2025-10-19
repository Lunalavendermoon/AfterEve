using System;
using UnityEngine;

public class Player_Dash : IPlayerState
{
    public float dashPower = 5f;
    public float dashDuration = 0.75f;
    public float dashStartTime;
    public Vector3 dashDirection;


    public void EnterState(PlayerController player)
    {
        dashStartTime = Time.time;

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
        if (Time.time - dashStartTime > dashDuration) player.ChangeState(new Player_Idle());
    }

    public void UpdateState(PlayerController player)
    {
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
    }
    
    public void ExitState(PlayerController player)
    {

    }
}
