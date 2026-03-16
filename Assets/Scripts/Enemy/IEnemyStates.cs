using UnityEngine;

public interface IEnemyStates
{
    public void EnterState(StandardEnemyBase enemy);
    public void UpdateState(StandardEnemyBase enemy);
    public void ExitState(StandardEnemyBase enemy);
}
