using System;
using UnityEngine;

public class Player_Move : IPlayerState
{
    public static event Action OnDisplaced;

    public static int speedCoefficient = 5;
    public void EnterState(PlayerController player, PlayerAttributes playerAttributes)
    {
        
    }

    public void CheckState(PlayerController player)
    {
        if (player.horizontalInput == 0 && player.verticalInput == 0) player.ChangeState(new Player_Idle());
        if (player.playerInput.Player.Dash.triggered) player.ChangeState(new Player_Dash());
    }

    public void UpdateState(PlayerController player)
    {
        Vector3 currentPosition = new Vector3();
        currentPosition.x += speedCoefficient * player.horizontalInput * player.playerAttributes.speed * Time.deltaTime;
        currentPosition.y += speedCoefficient * player.verticalInput * player.playerAttributes.speed * Time.deltaTime;

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
