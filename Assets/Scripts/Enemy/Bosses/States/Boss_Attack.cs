using UnityEngine;

public class Boss_Attack : IBossStates
{
    int attack_number;


    public Boss_Attack(int attack_number)
    {
        this.attack_number = attack_number;
    }
    public void EnterState(BossBehaviourBase boss)
    {
        //Debug.Log(attack_number);
        boss.agent.isStopped = true;
        boss.isAttacking = true;
        switch (attack_number)
        {
            case 1:
                boss.Attack1();
                
                break;
            case 2:
                boss.Attack2();
                break;
            case 3:
                boss.Attack3();
                break;
            case 4:
                boss.Attack4();
                break;
            case 5:
                boss.Attack5();
                break;
            default:
                Debug.LogError("Invalid attack number");
                break;
        }

        
    }

    public void UpdateState(BossBehaviourBase boss)
    {
        if (!boss.isAttacking)
        {
            boss.agent.isStopped = false;
            boss.ChangeState(new Boss_Cooldown(boss.cooldown_time));
            Debug.Log("Boss change to cooldown");
        }
    }

    public void ExitState(BossBehaviourBase boss)
    {
    }


}
