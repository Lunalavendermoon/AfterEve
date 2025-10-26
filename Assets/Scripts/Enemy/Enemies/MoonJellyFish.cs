using UnityEngine;

public class MoonJellyFish : EnemyBase
{
    public EnemyAttributes enemyAttributes;

    void Awake()
    {
        health = enemyAttributes.hitPoints;
        damage = enemyAttributes.enemyDamage;
        speed = enemyAttributes.speed;
        visibleRange = enemyAttributes.detection_radius;
        attackRange = enemyAttributes.attackRadius;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed;
    }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack(Transform target)
    {

    }

    public override void Die()
    {
    }

}
