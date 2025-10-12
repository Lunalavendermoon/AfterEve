using UnityEngine;
using UnityEngine.AI;


public abstract class EnemyBase : MonoBehaviour
{
    // Enemy attributes
    public int health;
    public int damage;
    public float speed;
    public float attackRange;
    public float visibleRange;
    NavMeshAgent agent;
    public IEnemyState default_enemy_state;
    protected IEnemyState current_enemy_state; 

    //Initializing agent and its default state
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        current_enemy_state= default_enemy_state;
    }


    public virtual void Update()
    {
        agent.speed = speed;
        current_enemy_state?.UpdateState(this);
        
    }


    // Actions
    public abstract void Attack(Transform target);
    
    public abstract void Die();

    public virtual bool InRange(Transform target);
    {
      float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= visibleRange)
        {
            return true;
        }
        return false;
    }

    public virtual void ChangeState(IEnemyState new_state)
    {
        current_enemy_state?.ExitState(this);
        current_enemy_state = new_state;
        current_enemy_state?.EnterState(this);
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
    }

    public virtual void Pathfinding(Transform target)
    {
        agent.SetDestination(target.position);
    }
}
