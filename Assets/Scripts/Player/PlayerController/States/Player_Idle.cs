using UnityEngine;
using UnityEngine.XR;

public class Player_Idle : IPlayerState
{
    public void EnterState(PlayerController player)
    {

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

    }
}
