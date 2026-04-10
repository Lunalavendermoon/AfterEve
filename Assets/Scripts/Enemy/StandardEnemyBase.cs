using Pathfinding;
using System;
using TMPro;
using UnityEngine;

public abstract class StandardEnemyBase : EnemyBase
{
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

        agent.maxSpeed = speed;

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
            spawner.AddPendingChestCoins(numShards);
            spawnerDeathNotified = true;
            spawner.EnemyDie(this);
        }
        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDie, this.transform.position);
        Destroy(gameObject);
    }

    public virtual bool InRange(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= enemyAttributes.detection_radius;
    }

    public virtual bool InAttackRange(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= enemyAttributes.attackRadius;
    }

    public virtual void ChangeState(IEnemyStates new_state)
    {
        current_enemy_state?.ExitState(this);
        current_enemy_state = new_state;
        current_enemy_state?.EnterState(this);
        Debug.Log($"{gameObject.name} changed to state: {current_enemy_state?.GetType().Name}");
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
            spawner.EnemyDie(this);
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
