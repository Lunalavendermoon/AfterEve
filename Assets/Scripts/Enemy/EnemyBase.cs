using UnityEngine;
using UnityEngine.AI;


public abstract class EnemyBase : MonoBehaviour
{
    public EnemyAttributes enemyAttributes;

    // Enemy attributes
    public int health;
    public int damage;
    public float speed;
    public float attackRange;
    public float visibleRange;
    public NavMeshAgent agent;
    public IEnemyStates default_enemy_state;
    protected IEnemyStates current_enemy_state; 

    //Initializing agent and its default state
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        current_enemy_state= default_enemy_state;
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
        
        Debug.Log($"{gameObject.name} is in state: {current_enemy_state?.GetType().Name}");
    }


    // Actions
    public abstract void Attack(Transform target);
    
    public abstract void Die();

    public virtual bool InRange(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= visibleRange;
    }

    public virtual void ChangeState(IEnemyStates new_state)
    {
        current_enemy_state?.ExitState(this);
        current_enemy_state = new_state;
        current_enemy_state?.EnterState(this);
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
    }

    public virtual void Pathfinding(Transform target)
    {
        agent.SetDestination(target.position);
    }
}
