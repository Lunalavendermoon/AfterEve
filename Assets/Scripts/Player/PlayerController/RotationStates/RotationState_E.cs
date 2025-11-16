using UnityEngine;

public class RotationState_E : IRotationState
{
    public void EnterState(PlayerController player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void UpdateState(PlayerController player)
    {
        player.CheckRotationTransition();
    }
}