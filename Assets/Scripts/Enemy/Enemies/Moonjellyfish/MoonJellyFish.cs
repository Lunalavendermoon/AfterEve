using Spine.Unity;
using UnityEngine;


/*    MoonJellyFish Enemy Class
    Inherits from EnemyBase and initializes attributes from EnemyAttributes ScriptableObject
*/
public class MoonJellyFish : StandardEnemyBase
{
    private SkeletonAnimation skeletonAnimation;

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

    public void Start()
    {
        base.Start();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
    }

    public override void Attack(Transform target)
    {



        // Enable hitbox for a short window
        EnableAttack();
        // Play SFX
        AudioManager.instance.PlayOneShot(FMODEvents.instance.blobAttack, this.transform.position);
        // Trigger animation 
        skeletonAnimation.AnimationState.SetAnimation(0, "Attack", false);
        skeletonAnimation.AnimationState.AddAnimation(0, "Idle", true, 0f);
        // animator.SetTrigger("Attack");
        Invoke(nameof(DisableAttack), 0.3f); // 0.3 is temp 
    }

    public override void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        base.TakeDamage(amount, dmgSource, dmgType);
        skeletonAnimation.AnimationState.SetAnimation(0, "Hit", false);
        skeletonAnimation.AnimationState.AddAnimation(0, "Idle", true, 0f);
    }

}
