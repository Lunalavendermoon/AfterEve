using UnityEngine;

public class Enemy_Idle :  IEnemyStates
{
    private float idleTime;
    private float elapsedTime;

    public Enemy_Idle(float idleDuration)
    {
        idleTime = idleDuration;
    }

    public Enemy_Idle() 
    {
        idleTime = 2f;
    }


    public void EnterState(EnemyBase enemy)
    {
        elapsedTime = 0f;
        enemy.speed = 0;

    }
    public void UpdateState(EnemyBase enemy)
    {

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= idleTime)
        {
            
            enemy.ChangeState(new Enemy_Wander());
        }
        if (enemy.InRange(PlayerController.instance.transform))
        {
            enemy.ChangeState(new Enemy_Chase());
        }


    }
    public void ExitState(EnemyBase enemy)
    {
        enemy.speed = enemy.enemyAttributes.speed;
    }
}
