using UnityEngine;
using UnityEngine.AI;

public class Enemy_Wander :  IEnemyStates
{
    private float wanderTime;
    private float wanderRadius;
    private float wanderTimer;

    public Enemy_Wander(float wanderDuration, float wanderRadius)
    {
        wanderTime = wanderDuration;
        this.wanderRadius = wanderRadius; // Example radius
    }

    public Enemy_Wander() 
    {
        wanderTime = 3f;
        this.wanderRadius = 15f;
    }


    public void EnterState(EnemyBase enemy)
    {
        wanderTimer = 0f;
        getRandomWanderPoint(enemy);

    }
    
    public void UpdateState(EnemyBase enemy)
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderTime)
        {
            getRandomWanderPoint(enemy);
            wanderTimer = 0f;
        }

        if (enemy.InRange(PlayerController.instance.transform))
        {
            enemy.ChangeState(new Enemy_Chase());
        }
        else if(enemy.agent.isStopped)
        {
           enemy.ChangeState(new Enemy_Idle());
        }


    }
    public void ExitState(EnemyBase enemy)
    {
        
    }

    void getRandomWanderPoint(EnemyBase enemy)
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += enemy.transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1);
        enemy.agent.SetDestination(navHit.position);
    }
}
