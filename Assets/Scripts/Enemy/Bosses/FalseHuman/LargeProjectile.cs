using Pathfinding;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LargeProjectile : EnemyBase
{
    float timer = 0;
    bool isHaste = false;

    void Awake()
    {
        health = enemyAttributes.maxHitPoints;
        //damage = enemyAttributes.damage;
        speed = enemyAttributes.speed;
        //visibleRange = enemyAttributes.detection_radius;
        //attackRange = enemyAttributes.attackRadius;

        default_enemy_state = new Enemy_Chase();

        //attackCooldown = enemyAttributes.attackRate;

    }

    // Update is called once per frame
    void Update()
    {
        current_enemy_state?.UpdateState(this);
        if (timer < 10)
        {
            timer += Time.deltaTime;
            return;
        }
        if (!isHaste)
        {
            isHaste = true;
            enemyEffectManager.AddEffect(new Haste_Effect(20,4.0f));
        }
    }

 

    public override void Attack(Transform target)
    {
        PlayerController.instance.TakeDamage(health, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Spiritual);
        Destroy(this);

    }
}


