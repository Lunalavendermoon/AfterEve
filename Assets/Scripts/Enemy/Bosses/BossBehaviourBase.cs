using Pathfinding;
using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public abstract class BossBehaviourBase : MonoBehaviour
{
    public EnemyAttributes baseEnemyAttributes;

    public EnemyAttributes enemyAttributes;

    public EnemyEffectManager enemyEffectManager;

    public Chest_base chest;
    public EnemySpawnerScript spawner;

    // Enemy attributes
    public int health;
    //public int damage;
    public float speed;
    //public float attackRange;
    //public float visibleRange;
    //public float attackCooldown;

    //Pathfinding agent
    public AIPath agent;
    public AIDestinationSetter destinationSetter;

    //Helper variables
    public GameObject attackHitbox;
    public IBossStates default_enemy_state;
    protected IBossStates current_enemy_state;
    public bool isAttacking = false;
    public float cooldown_time;

    //Drops
    public GameObject Drop;

    public float attack_timer = 1.0f;

    // event for enemy dying
    public static event Action<DamageInstance, EnemyBase> OnEnemyDamageTaken;

    // event for enemy dying
    public static event Action<DamageInstance, EnemyBase> OnEnemyDeath;

    private Transform tempTarget; // Temporary target for pathfinding to a position

    public GameObject floatingTextPrefab;

    public float[] attackProbalities;

    // 2 states : in cooldown and attack 
    // during in cooldown will use the movement func
    // after cooldown will roll a dice to see which of the 5 attacks 
    // probabilities for each attack will be defined in the particular boss script
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        current_enemy_state = default_enemy_state;
        current_enemy_state.EnterState(this);
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        current_enemy_state?.UpdateState(this);
    }

    public abstract void Movement();
    public abstract void Attack1();
    public abstract void Attack2();
    public abstract void Attack3();
    public abstract void Attack4();

    public abstract void Attack5();

    public virtual void ChangeState(IBossStates new_state)
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

    }
    public virtual void DisableAttack()
    {
        attackHitbox.SetActive(false);
        isAttacking = false;
    }

    protected void ShowFloatingText(int damageAfterReduction)
    {
        if (floatingTextPrefab != null)
        {
            Debug.Log("Showing floating text for damage: " + damageAfterReduction);
            GameObject floatingText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
            floatingText.GetComponentInChildren<TextMeshPro>().text = damageAfterReduction.ToString();

        }
    }

    public virtual void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        int damageAfterReduction = Mathf.CeilToInt(amount * (1 - (enemyAttributes.basicDefense / (enemyAttributes.basicDefense + 100))));
        health -= damageAfterReduction;

        //OnEnemyDamageTaken?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);

        // Damage numbers
        ShowFloatingText(damageAfterReduction);
        Debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
        if (health <= 0)
        {
            // TODO: set hitWeakPoint to true/false depending on whether weak point was hit with the current attack
            //OnEnemyDeath?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);
            Die();
        }
    }

    public virtual void Die()
    {
        //EnemyItemDrops.ItemDrop(PlayerController.instance.playerAttributes.luck, elite, chest);
        //spawner.EnemyDie(this);
        Destroy(gameObject);
    }


    public virtual void Pathfinding(Transform target)
    {
        Debug.Log("Pathfinding start");
        destinationSetter.target = target;
    }

    public virtual int ChooseAttack()
    {
        float choice = UnityEngine.Random.Range(0f, 1f)*100;
        float attackProbailitySum = 0f;
        for (int i = 0; i < attackProbalities.Length; i++)
        {
            if (attackProbalities[i] > 0)
            {
                attackProbailitySum += attackProbalities[i];
                if (choice <= attackProbailitySum)
                {
                    Debug.Log($"Chose attack {i+1} with choice value {choice} and cumulative probability {attackProbailitySum}");
                    return i+1;
                }
            }
        }
        Debug.Log("Attack choice failed, defaulting to attack 1");
        return 1; //default to attack 1
    }




}
