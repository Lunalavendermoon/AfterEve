using UnityEngine;

public class RotationState_S : IRotationState
{
    public void EnterState(PlayerController player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 270);
    }

    public void UpdateState(PlayerController player)
    {
        player.CheckRotationTransition();
    }
}