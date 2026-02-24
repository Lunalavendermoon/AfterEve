using UnityEngine;

public interface IPlayerState
{
    public static int speedCoefficient = 5;
    public void EnterState(PlayerController player);
    public void CheckState(PlayerController player);
    public void UpdateState(PlayerController player);

    // New: physics-step movement should be done here (FixedUpdate).
    public void FixedUpdateState(PlayerController player);

    public void ExitState(PlayerController player);
}
