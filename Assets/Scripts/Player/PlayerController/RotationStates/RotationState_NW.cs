using UnityEngine;

public class RotationState_NW : IRotationState
{
    public void EnterState(PlayerController player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 135);
    }

    public void UpdateState(PlayerController player)
    {
        player.CheckRotationTransition();
    }
}