using UnityEngine;

public class Enemy_Attack : IEnemyStates
{
    public Enemy_Attack()
    {

    }

    public void EnterState(EnemyBase enemy)
    {

    }

    public void UpdateState(EnemyBase enemy)
    {
        enemy.Attack(PlayerController.instance.transform);
        enemy.ChangeState(new Enemy_Chase());
    }
    
    public void ExitState(EnemyBase enemy)
    {
        
    }
}