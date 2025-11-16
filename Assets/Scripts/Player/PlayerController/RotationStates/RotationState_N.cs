using UnityEngine;

public class RotationState_N : IRotationState
{

    public void EnterState(PlayerController player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    public void UpdateState(PlayerController player)
    {
        player.CheckRotationTransition();
    }
}