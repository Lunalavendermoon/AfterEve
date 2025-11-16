using UnityEngine;

public class RotationState_SE : IRotationState
{

    public void EnterState(PlayerController player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 315);
    }

    public void UpdateState(PlayerController player)
    {
        player.CheckRotationTransition();
    }
}