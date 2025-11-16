using UnityEngine;

public class RotationState_NE : IRotationState
{
    public void EnterState(PlayerController player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 45);
    }

    public void UpdateState(PlayerController player)
    {
        player.CheckRotationTransition();
    }
}