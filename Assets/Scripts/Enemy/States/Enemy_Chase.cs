using UnityEngine;
using UnityEngine.AI;

public class Enemy_Chase : IEnemyStates
{

    // Time to wait between attacks
    private float attackCooldown;
    private float attackTimer;

    public Enemy_Chase( float attackCooldown)
    {
        this.attackCooldown = attackCooldown;
    }

    public Enemy_Chase()
    {
        attackCooldown = 2f;
    }

    public void EnterState(StandardEnemyBase enemy)
    {
        
        attackTimer = 0f;
        enemy.agent.isStopped = false;
    }

    public void UpdateState(StandardEnemyBase enemy)
    {
        if (enemy.enemyAttributes.isParalyzed)
        {
            enemy.ChangeState(new Enemy_Idle());
            return;
        }
        
        getChasePoint(enemy);

        attackTimer += Time.deltaTime;

        if (enemy.InAttackRange(PlayerController.instance.transform))
        {
           enemy.ChangeState(new Enemy_Attack());
        }
        
    }

    void getChasePoint(StandardEnemyBase enemy)
    {
        enemy.Pathfinding(PlayerController.instance.transform);
    }
    
    public void ExitState(StandardEnemyBase enemy)
    {
        
    }
}