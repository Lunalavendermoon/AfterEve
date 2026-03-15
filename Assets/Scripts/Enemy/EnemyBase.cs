using Pathfinding;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;


public abstract class EnemyBase : MonoBehaviour
{
    public EnemyAttributes baseEnemyAttributes;

    public EnemyAttributes enemyAttributes;

    public EnemyEffectManager enemyEffectManager;
    public EnemySpawnerScript spawner;
    protected Transform tempTarget;
    // Enemy attributes
    public int health;
    public float speed;


    //Pathfinding agent
    public AIPath agent;
    public AIDestinationSetter destinationSetter;



    public GameObject floatingTextPrefab;
    public Rigidbody2D rb;


    // event for enemy dying
    public static event Action<DamageInstance, EnemyBase> OnEnemyDamageTaken;

    // event for enemy dying
    public static event Action<DamageInstance, EnemyBase> OnEnemyDeath;

    //lovers clone
    protected float marked = 1;

    //chaining
    protected bool isChained;
    protected float chainTime;


    public bool IsChained()
    {
        return isChained;
    }

    public void Chain(float f)
    {
        isChained = true;
        chainTime = Time.time + f;
    }

    public void Mark(float f)
    {
        marked = f;
    }

    public virtual void Die()
    {

    }





    public virtual void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        amount = (int)(amount * marked);

        int damageAfterReduction = Mathf.CeilToInt(amount * (1 - (enemyAttributes.basicDefense / (enemyAttributes.basicDefense + 100))));
        health -= damageAfterReduction;

        OnEnemyDamageTaken?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyTakeDamage, this.transform.position);

        // Damage numbers
        ShowFloatingText(damageAfterReduction);
        Debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
        if (health <= 0)
        {
            // TODO: set hitWeakPoint to true/false depending on whether weak point was hit with the current attack
            OnEnemyDeath?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);
            Die();
        }
    }

    // Can make this virtual and override for bosses w/ multiple phases if needed.
    public bool IsAlive()
    {
        return health > 0;
    }

    public virtual void Heal(int amount)
    {
        health = Math.Clamp(health + amount, 0, enemyAttributes.maxHitPoints);
        Debug.Log($"{gameObject.name} healed {amount}, current health: {health}");
    }

    public virtual void Pathfinding(Transform target)
    {
        destinationSetter.target = target;
    }



    protected void ShowFloatingText(int damageAfterReduction)
    {
        if (floatingTextPrefab != null)
        {
            //Debug.Log("Showing floating text for damage: " + damageAfterReduction);
            GameObject floatingText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            var tmp = floatingText.GetComponentInChildren<TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = damageAfterReduction.ToString();
            }

        }
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

    private void OnDestroy()
    {

    }
}
