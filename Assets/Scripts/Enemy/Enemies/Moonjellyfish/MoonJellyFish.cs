using UnityEngine;


/*    MoonJellyFish Enemy Class
    Inherits from EnemyBase and initializes attributes from EnemyAttributes ScriptableObject
*/
public class MoonJellyFish : EnemyBase
{
    [SerializeField] private float wanderRadius = 7.5f;
    [SerializeField] private float wanderTime = 3f;
    void Awake()
    {
        health = enemyAttributes.hitPoints;
        damage = enemyAttributes.damage;
        speed = enemyAttributes.speed;
        visibleRange = enemyAttributes.detection_radius;
        attackRange = enemyAttributes.attackRadius;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed;
        default_enemy_state = new Enemy_Wander(wanderRadius, wanderTime);

        attackCooldown = enemyAttributes.attackRate;

    }

  

    public override void Attack(Transform target)
    {



        // Enable hitbox for a short window
        EnableAttack();
        // Trigger animation 
        // animator.SetTrigger("Attack");
        Invoke(nameof(DisableAttack), 0.3f); // 0.3 is temp 
    }

    


}
