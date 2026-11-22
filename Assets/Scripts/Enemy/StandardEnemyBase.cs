using Pathfinding;
using System;
using TMPro;
using UnityEngine;

public abstract class StandardEnemyBase : EnemyBase
{
    private const float DetectionRadiusMultiplier = 1.25f;
    private const float PursuitRadiusMultiplier = 1.5f;
    private const float PursuitSpeedMultiplier = 1.1f;
    private const float OffscreenPursuitSpeedMultiplier = 2.5f;
    private const float ViewportMargin = 0.05f;

    public Chest_base chest;


    //Helper variables
    public GameObject attackHitbox;
    [Tooltip("Force applied to player when this enemy hits with melee (0 = no knockback).")]
    public float knockbackForceOnHit = 0f;
    public IEnemyStates default_enemy_state;
    protected IEnemyStates current_enemy_state;
    public bool isAttacking = false;
    public bool attackHitConnected;
    //Drops
    public GameObject Drop;

    public float attack_timer = 1.0f;



    private bool spawnerDeathNotified;


    public bool givesRewards = true; // no rewards if spawned by boss

    [Header("Visual Facing")]
    [SerializeField] private bool lockRotation = true;
    [SerializeField] private bool flipToFacePlayer = true;



    public bool elite = false;

    protected virtual bool IsPursuing => current_enemy_state is Enemy_Chase;
    public bool ApproachFromOffscreen { get; set; }

    private bool IsOffscreen()
    {
        Camera camera = Camera.main;
        if (camera == null)
            return false;

        Vector3 viewportPosition = camera.WorldToViewportPoint(transform.position);
        return viewportPosition.z < 0f ||
            viewportPosition.x < -ViewportMargin || viewportPosition.x > 1f + ViewportMargin ||
            viewportPosition.y < -ViewportMargin || viewportPosition.y > 1f + ViewportMargin;
    }


    //Initializing agent and its default state
    public void Start()
    {
        agent = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        if (agent != null)
        {
            agent.enableRotation = false;
            agent.updateRotation = false;
        }

        current_enemy_state = default_enemy_state;
        current_enemy_state.EnterState(this);
        isChained = false;
    }


    private void Update()
    {
        if (!IsAlive())
            return;

        if (lockRotation)
        {
            var e = transform.eulerAngles;
            if (e.z != 0f)
            {
                e.z = 0f;
                transform.eulerAngles = e;
            }
        }

        if (flipToFacePlayer && PlayerController.instance != null)
        {
            float dx = PlayerController.instance.transform.position.x - transform.position.x;
            if (dx != 0f)
            {
                Vector3 s = transform.localScale;
                float absX = Mathf.Abs(s.x);
                s.x = dx > 0f ? -absX : absX;
                transform.localScale = s;
            }
        }

        float speedMultiplier = 1f;
        if (IsPursuing)
            speedMultiplier = ApproachFromOffscreen && IsOffscreen()
                ? OffscreenPursuitSpeedMultiplier
                : PursuitSpeedMultiplier;
        agent.maxSpeed = speed * speedMultiplier;

        current_enemy_state?.UpdateState(this);

        // if (enemyAttributes.isParalyzed)
        // {
        //     // enemy can't do anything if it's paralyzed
        //     current_enemy_state = new Enemy_Idle();
        // }
        // else
        // {
        //     current_enemy_state?.UpdateState(this);
        // }

        //Debug.Log($"{gameObject.name} is in state: {current_enemy_state?.GetType().Name}");

        if (isChained)
            if (Time.time > chainTime)
                isChained = false;

        EnemyUpdate();

    }

    public virtual void EnemyUpdate()
    {

    }

    // Actions
    public abstract void Attack(Transform target);

    public override void Die()
    {
        if (givesRewards)
        {
            int numShards = EnemyItemDrops.CalculateShardDrop(PlayerController.instance.playerAttributes.GetAdjustedLuck(), elite);
            int numTarots = EnemyItemDrops.CalculateTarotDrop(PlayerController.instance.playerAttributes.GetAdjustedLuck(), elite);
            spawner.AddPendingChestTarots(numTarots);
            spawner.AddPendingChestCoins(numShards);
            spawnerDeathNotified = true;
            spawner.EnemyDie(this);
        }
        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDie, this.transform.position);
        Destroy(gameObject);
    }

    public virtual bool InRange(Transform target)
    {
        return WithinDetectionRadius(target, DetectionRadiusMultiplier) ||
            (ApproachFromOffscreen && IsOffscreen());
    }

    protected bool InPursuitRange(Transform target)
    {
        return WithinDetectionRadius(target, PursuitRadiusMultiplier);
    }

    private bool WithinDetectionRadius(Transform target, float multiplier)
    {
        float distance = Vector2.Distance(transform.position, target.position);
        return distance <= enemyAttributes.detection_radius * multiplier;
    }

    public virtual bool InAttackRange(Transform target)
    {
        float distance = Vector2.Distance(transform.position, target.position);
        return distance <= enemyAttributes.attackRadius;
    }

    public virtual void ChangeState(IEnemyStates new_state)
    {
        current_enemy_state?.ExitState(this);
        current_enemy_state = new_state;
        current_enemy_state?.EnterState(this);
        //Debug.Log($"{gameObject.name} changed to state: {current_enemy_state?.GetType().Name}");
    }




    public virtual void EnableAttack()
    {
        attackHitbox.SetActive(true);
        isAttacking = true;
        attackHitConnected = false;

    }
    public virtual void DisableAttack()
    {
        attackHitbox.SetActive(false);
        isAttacking = false;
    }




    private void OnDestroy()
    {
        if (!spawnerDeathNotified && givesRewards && spawner != null)
        {
            spawnerDeathNotified = true;
            // spawner.EnemyDie(this);
        }

        if (destinationSetter != null && destinationSetter.target == tempTarget)
        {
            destinationSetter.target = null;
        }

        if (tempTarget != null)
        {
            Destroy(tempTarget.gameObject);
            tempTarget = null;
        }
    }
}
