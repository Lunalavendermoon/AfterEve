using UnityEngine;

public interface IPlayerState
{
    public static int speedCoefficient = 5;

    public void EnterState(PlayerController player);
    public void CheckState(PlayerController player);
    public void UpdateState(PlayerController player);
    public void ExitState(PlayerController player);
}
