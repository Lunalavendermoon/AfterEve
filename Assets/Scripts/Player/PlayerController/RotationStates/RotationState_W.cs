using UnityEngine;

public class RotationState_W : IRotationState
{
    public void EnterState(PlayerController player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    public void UpdateState(PlayerController player)
    {
        player.CheckRotationTransition();
    }
}