using UnityEngine;

public class RotationState_SW : IRotationState
{

    public void EnterState(PlayerController player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 225);
    }

    public void UpdateState(PlayerController player)
    {
        player.CheckRotationTransition();
    }
}