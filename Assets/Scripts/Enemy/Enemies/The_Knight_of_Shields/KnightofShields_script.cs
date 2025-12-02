using UnityEngine;

public class KnightofShields_script : EnemyBase
{
    [SerializeField] private float wanderRadius = 7.5f;
    [SerializeField] private float wanderTime = 3f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 1f;

    void Awake()
    {
        health = enemyAttributes.maxHitPoints;
        //damage = enemyAttributes.damage;
        speed = enemyAttributes.speed;
        //visibleRange = enemyAttributes.detection_radius;
        //attackRange = enemyAttributes.attackRadius;

        default_enemy_state = new Enemy_Wander(wanderRadius, wanderTime);

        //attackCooldown = enemyAttributes.attackRate;

    }


    public override void Attack(Transform target)
    {
        EnableAttack();
        // Trigger animation 
        // animator.SetTrigger("Attack");
        Invoke(nameof(DisableAttack), 0.5f);

    }
}
