using UnityEngine;

public interface IPlayerState
{
    public void EnterState(PlayerController player, PlayerAttributes playerAttributes);
    public void CheckState(PlayerController player);
    public void UpdateState(PlayerController player);
    public void ExitState(PlayerController player);
}
