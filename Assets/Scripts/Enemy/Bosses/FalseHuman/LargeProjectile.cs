using Pathfinding;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LargeProjectile : StandardEnemyBase
{
    private const float InitialSpeed = 1f;
    private const float HasteSpeed = 4f;
    private const float HasteDelay = 10f;
    private float timer;
    private bool hasAppliedHaste;
    void Awake()
    {
        if (baseEnemyAttributes != null)
        {
            enemyAttributes = Instantiate(baseEnemyAttributes);
            health = Mathf.Max(1000, enemyAttributes.maxHitPoints);
            speed = InitialSpeed;
        }
        else
        {
            health = 1000;
            speed = InitialSpeed;
        }
        default_enemy_state = new Enemy_Chase();
        attack_timer = 0f;
    }

    private new void Start()
    {
        base.Start(); // or StandardEnemyBase Start – gets agent, sets enableRotation/updateRotation = false
        if (agent != null)
        {
            agent.enableRotation = true;
            agent.updateRotation = true;
        }
    }
    public override void EnemyUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= HasteDelay && !hasAppliedHaste)
        {
            hasAppliedHaste = true;
            speed = HasteSpeed;
        }
    }
    public override void Attack(Transform target)
    {
        if (PlayerController.instance != null)
            PlayerController.instance.TakeDamage(health, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Spiritual);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            if (PlayerController.instance != null)
                PlayerController.instance.TakeDamage(health, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Spiritual);
            Destroy(gameObject);
        }
    }
}


