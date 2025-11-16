using UnityEngine;

public interface IRotationState
{
    void EnterState(PlayerController player);
    void UpdateState(PlayerController player);

}