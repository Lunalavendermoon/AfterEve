
using UnityEngine;

public interface IBossStates
{
    public void EnterState(BossBehaviourBase boss);
    public void UpdateState(BossBehaviourBase boss);
    public void ExitState(BossBehaviourBase boss);
}
