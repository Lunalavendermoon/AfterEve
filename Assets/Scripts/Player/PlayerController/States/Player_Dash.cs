using System;
using UnityEngine;

public class Player_Dash : IPlayerState
{
    public float dashPower = 5f;
    public float dashDuration = 0.75f;
    public float dashStartTime;
    public Vector3 dashDirection;


    public static event Action OnDash;
    public static event Action OnDisplaced;


    public void EnterState(PlayerController player)
    {
        OnDash?.Invoke();
        dashStartTime = Time.time;

        if (player.horizontalInput != 0 && player.verticalInput != 0)
        {
            dashDirection = new Vector3(player.horizontalInput / (float) Math.Sqrt(2), player.verticalInput / (float) Math.Sqrt(2), 0);
        }
        else
        {
            dashDirection = new Vector3(player.horizontalInput, player.verticalInput, 0);
        }   

        Debug.Log("dash direction: " + dashDirection);
    }

    public void CheckState(PlayerController player)
    {
        if (Time.time - dashStartTime > dashDuration) player.ChangeState(new Player_Idle());
    }

    public void UpdateState(PlayerController player)
    {
        float deceleration = dashDuration - (Time.time - dashStartTime);
        deceleration *= deceleration;
        player.transform.position += IPlayerState.speedCoefficient * player.playerAttributes.speed * dashPower * deceleration * Time.deltaTime * dashDirection;
        Debug.Log("speed: " + player.playerAttributes.speed + " dash power: " + dashPower + " deceleration: " + deceleration);

        // allow movement as well
        Vector3 currentPosition = new Vector3();
        currentPosition.x += IPlayerState.speedCoefficient * player.horizontalInput * player.playerAttributes.speed * Time.deltaTime;
        currentPosition.y += IPlayerState.speedCoefficient * player.verticalInput * player.playerAttributes.speed * Time.deltaTime;

        if (player.horizontalInput != 0 && player.verticalInput != 0)
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
