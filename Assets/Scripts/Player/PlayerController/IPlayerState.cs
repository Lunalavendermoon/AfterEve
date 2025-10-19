using UnityEngine;

public interface IPlayerState
{
    public void EnterState(PlayerController player);
    public void CheckState(PlayerController player);
    public void UpdateState(PlayerController player);
    public void ExitState(PlayerController player);
}
