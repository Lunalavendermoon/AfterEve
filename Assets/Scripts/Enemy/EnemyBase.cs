using System;
using UnityEngine;
using UnityEngine.AI;


public abstract class EnemyBase : MonoBehaviour
{
    public EnemyAttributes enemyAttributes;

    public EnemyEffectManager enemyEffectManager;

    // Enemy attributes
    public int health;
    public int damage;
    public float speed;
    public float attackRange;
    public float visibleRange;
    public float attackCooldown;

    //Pathfinding agent
    public NavMeshAgent agent;
    
    //Helper variables
    public GameObject attackHitbox;
    public IEnemyStates default_enemy_state;
    protected IEnemyStates current_enemy_state;
    public bool isAttacking = false;

    public float attack_timer = 1.0f;

    // event for enemy dying
    public static event Action<DamageInstance, EnemyBase> OnEnemyDeath;

    //Initializing agent and its default state
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        current_enemy_state = default_enemy_state;
    }


    public virtual void Update()
    {
        agent.speed = speed;

        if (enemyAttributes.isParalyzed)
        {
            // enemy can't do anything if it's paralyzed
            current_enemy_state = new Enemy_Idle();
        }
        else
        {
            current_enemy_state?.UpdateState(this);
        }

        //Debug.Log($"{gameObject.name} is in state: {current_enemy_state?.GetType().Name}");
    }


    // Actions
    public abstract void Attack(Transform target);

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual bool InRange(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= visibleRange;
    }

    public virtual bool InAttackRange(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= attackRange;
    }

    public virtual void ChangeState(IEnemyStates new_state)
    {
        current_enemy_state?.ExitState(this);
        current_enemy_state = new_state;
        current_enemy_state?.EnterState(this);
        Debug.Log($"{gameObject.name} changed to state: {current_enemy_state?.GetType().Name}");
    }

    public virtual void TakeDamage(int amount, DamageInstance dmgInstance)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
        if (health <= 0)
        {
            OnEnemyDeath.Invoke(dmgInstance, this);
            Die();
        }
    }

    public virtual void Pathfinding(Transform target)
    {
        agent.SetDestination(target.position);
    }

    public virtual void EnableAttack()
    {
        attackHitbox.SetActive(true);
        isAttacking = true;

    }
    public virtual void DisableAttack()
    {
        attackHitbox.SetActive(false);
        isAttacking = false;
    }
}
