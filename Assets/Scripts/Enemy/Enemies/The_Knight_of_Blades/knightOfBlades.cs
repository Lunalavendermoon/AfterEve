using System.Collections;
using UnityEngine;

public class knightOfBlades : EnemyBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        
        StartCoroutine(DashAttack());
    }





    private IEnumerator DashAttack()
    {
        // Enable hitbox for duration of dash
        EnableAttack();

        float elapsed = 0f;

        Vector3 dashDirection = transform.up; 

        while (elapsed < dashDuration)
        {
            // Move enemy forward
            transform.position += dashDirection * dashSpeed * Time.deltaTime;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Disable hitbox after dash
        DisableAttack();


    }
}
