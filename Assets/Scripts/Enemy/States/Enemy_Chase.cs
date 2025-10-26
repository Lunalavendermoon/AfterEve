using UnityEngine;
using UnityEngine.AI;

public class Enemy_Chase : IEnemyStates
{
    // Time to keep chasing the player after player is out of range
    private float chaseDuration;

    private float chaseTimer;

    // Time to wait between attacks
    private float attackCooldown;
    private float attackTimer;

    public Enemy_Chase(float chaseDuration, float attackCooldown)
    {
        this.chaseDuration = chaseDuration;
        this.attackCooldown = attackCooldown;
    }

    public Enemy_Chase()
    {
        chaseDuration = 3f;
        attackCooldown = 2f;
    }

    public void EnterState(EnemyBase enemy)
    {
        chaseTimer = 0f;
        attackTimer = 0f;
    }

    public void UpdateState(EnemyBase enemy)
    {
        getChasePoint(enemy);

        attackTimer += Time.deltaTime;

        if (!enemy.InRange(PlayerController.instance.transform))
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= chaseDuration)
            {
                enemy.ChangeState(new Enemy_Idle());
            }
        }
        else
        {
            chaseTimer = 0f;

            if (attackTimer >= attackCooldown)
            {
                enemy.ChangeState(new Enemy_Attack());
            }
        }
    }

    void getChasePoint(EnemyBase enemy)
    {
        enemy.Pathfinding(PlayerController.instance.transform);
    }
    
    public void ExitState(EnemyBase enemy)
    {
        
    }
}