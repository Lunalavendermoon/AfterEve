using Pathfinding;
using System;
using System.IO;
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
    public AIPath agent;
    public AIDestinationSetter destinationSetter;

    //Helper variables
    public GameObject attackHitbox;
    public IEnemyStates default_enemy_state;
    protected IEnemyStates current_enemy_state;
    public bool isAttacking = false;

    public float attack_timer = 1.0f;

    // event for enemy dying
    public static event Action<DamageInstance, EnemyBase> OnEnemyDeath;

    private Transform tempTarget; // Temporary target for pathfinding to a position

    //Initializing agent and its default state
    public virtual void Start()
    {
        agent = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        current_enemy_state = default_enemy_state;
    }


    public virtual void Update()
    {
        agent.maxSpeed = speed;

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

    public virtual void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
        if (health <= 0)
        {
            // TODO: factor in enemy damage-reduction
            // TODO: set hitWeakPoint to true/false depending on whether weak point was hit with the current attack
            OnEnemyDeath.Invoke(new DamageInstance(dmgSource, dmgType, amount, amount), this);
            Die();
        }
    }

    public virtual void Pathfinding(Transform target)
    {
        destinationSetter.target = target;
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

    public virtual void Pathfinding(Vector3 targetPosition)
    {
        if (destinationSetter == null) return;

        // Create a temporary GameObject internally
        if (tempTarget == null)
        {
            GameObject go = new GameObject($"{gameObject.name}_TempTarget");
            tempTarget = go.transform;
        }

        tempTarget.position = targetPosition;
        destinationSetter.target = tempTarget;
    }

    
}
