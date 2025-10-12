using UnityEngine;
using UnityEngine.AI;

public class Enemy_Chase : IEnemyStates
{
    // Time to keep chasing the player after player is out of range
    private float chaseDuration;

    private float chaseTimer;

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
        attackTimer = attackCooldown;
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
        } else
        {
            chaseTimer = 0f;

            if (attackTimer >= attackCooldown)
            {
                enemy.Attack(PlayerController.instance.transform);
                attackTimer = 0f;
            }
        }
    }

    void getChasePoint(EnemyBase enemy)
    {
        enemy.Pathfinding(PlayerController.instance.transform.position);
    }
    
    public void ExitState(EnemyBase enemy)
    {
        
    }
}