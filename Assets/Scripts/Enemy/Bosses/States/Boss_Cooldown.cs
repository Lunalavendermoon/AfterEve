using UnityEngine;

public class Boss_Cooldown : IBossStates
{
    float cooldown_time;

    
    public Boss_Cooldown(float cooldown_time)
    {
        this.cooldown_time = cooldown_time;
        
    }

    public void EnterState(BossBehaviourBase boss)
    {
        boss.Movement();
    }

    public void UpdateState(BossBehaviourBase boss)
    {
        cooldown_time-=Time.deltaTime;
        if (cooldown_time < 0)
        {
            cooldown_time = 0;

            boss.ChangeState(new Boss_Attack(boss.ChooseAttack()));
        }

    }

    public void ExitState(BossBehaviourBase boss)
    {
    }
}
