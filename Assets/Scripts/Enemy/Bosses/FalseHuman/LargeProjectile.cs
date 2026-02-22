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
        speed = enemyAttributes.speed;
        default_enemy_state = new Enemy_Chase();
        attack_timer = 0.0f;

    }

    // Update is called once per frame
    public override void EnemyUpdate()
    {
        print("In range: " + Vector3.Distance(transform.position, PlayerController.instance.transform.position));
        if (timer < 10)
        {
            timer += Time.deltaTime;
            return;
        }
        if (!isHaste)
        {
            isHaste = true;
            //enemyEffectManager.AddBuff(new Haste_Effect(20,4.0f));
        }

        
    }

 

    public override void Attack(Transform target)
    {
        Debug.Log("Attack Mode");
        PlayerController.instance.TakeDamage(health, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Spiritual);
        Destroy(gameObject);
        Debug.Log("Large Projectile deleted.");

    }
}


