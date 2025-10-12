using UnityEngine;

public class Enemy_Wander :  IEnemyStates
{
    private float wanderTime;
    private float elapsedTime;
    private float wanderRadius;

    public Enemy_Wander(float wanderDuration, float wanderRadius)
    {
        wanderTime = wanderDuration;
        this.wanderRadius = wanderRadius; // Example radius
    }

    public Enemy_Wander() 
    {
        wanderTime = 10f;
        this.wanderRadius = 3f;
    }


    public void EnterState(EnemyBase enemy)
    {
        elapsedTime = 0f;
        

    }
    public void UpdateState(EnemyBase enemy)
    {
        getRandomWanderPoint(enemy);
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
