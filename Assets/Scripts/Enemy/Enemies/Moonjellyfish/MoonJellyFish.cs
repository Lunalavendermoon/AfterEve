using UnityEngine;


/*    MoonJellyFish Enemy Class
    Inherits from EnemyBase and initializes attributes from EnemyAttributes ScriptableObject
*/
public class MoonJellyFish : StandardEnemyBase
{
    [SerializeField] private float wanderRadius = 7.5f;
    [SerializeField] private float wanderTime = 3f;
    private void Awake()
    {
        transform.rotation = Quaternion.identity;

        if (baseEnemyAttributes != null)
        {
            enemyAttributes = Instantiate(baseEnemyAttributes);

            int maxHp = Mathf.Max(1, enemyAttributes.maxHitPoints);
            health = maxHp;
            speed = enemyAttributes.speed;
        }
        else
        {
            health = Mathf.Max(1, health);
        }
        
        default_enemy_state = new Enemy_Wander(wanderTime, wanderRadius);

        //attackCooldown = enemyAttributes.attackRate;

    }

  

    public override void Attack(Transform target)
    {



        // Enable hitbox for a short window
        EnableAttack();
        // Play SFX
        AudioManager.instance.PlayOneShot(FMODEvents.instance.blobAttack, this.transform.position);
        // Trigger animation 
        // animator.SetTrigger("Attack");
        Invoke(nameof(DisableAttack), 0.3f); // 0.3 is temp 
    }

    


}
