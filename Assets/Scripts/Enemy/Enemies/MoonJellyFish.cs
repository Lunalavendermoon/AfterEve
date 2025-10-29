using UnityEngine;


/*    MoonJellyFish Enemy Class
    Inherits from EnemyBase and initializes attributes from EnemyAttributes ScriptableObject
*/
public class MoonJellyFish : EnemyBase
{
    void Awake()
    {
        health = enemyAttributes.hitPoints;
        damage = enemyAttributes.damage;
        speed = enemyAttributes.speed;
        visibleRange = enemyAttributes.detection_radius;
        attackRange = enemyAttributes.attackRadius;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed;
        default_enemy_state = new Enemy_Wander(10f, 3f);
    }

  

    public override void Attack(Transform target)
    {
        //TODO


    }

    public override void Die()
    {
        //TODO
    }

}
