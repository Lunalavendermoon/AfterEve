using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy_Attack : IEnemyStates
{
    public Enemy_Attack()
    {

    }

    public void EnterState(EnemyBase enemy)
    {
        enemy.agent.isStopped = true;
        Debug.Log("In attack range");
    }

    public void UpdateState(EnemyBase enemy)
    {
        Transform target = PlayerController.instance.transform;
        if (enemy.isAttacking) return; // already attacking

        if (enemy.enemyAttributes.isParalyzed)
        {
            enemy.ChangeState(new Enemy_Idle());
            return;
        }

        if (!enemy.InAttackRange(PlayerController.instance.transform))
        {
            enemy.ChangeState(new Enemy_Chase());
            return;
        }

        

        if (enemy.attack_timer > 0 )
        {
            Vector3 dir = (target.position - enemy.transform.position).normalized;
            
            
            if (dir.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion lookRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
            enemy.attack_timer -= Time.deltaTime;
            return; // waiting for cooldown
        }
        enemy.Attack(PlayerController.instance.transform);

        enemy.attack_timer = enemy.enemyAttributes.attackRate;
    }
    
    public void ExitState(EnemyBase enemy)
    {
        
    }
}