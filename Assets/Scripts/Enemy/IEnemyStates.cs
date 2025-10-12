using UnityEngine;

public interface IEnemyStates
{
    public void EnterState(EnemyBase enemy);
    public void UpdateState(EnemyBase enemy);
    public void ExitState(EnemyBase enemy);
}
