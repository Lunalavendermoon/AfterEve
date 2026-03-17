using System.Collections;
using UnityEngine;

public class knightOfBlades : StandardEnemyBase, IKnightWithWeakPoint
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float wanderRadius = 7.5f;
    [SerializeField] private float wanderTime = 3f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 1f;
    private bool weakPointHitBySpiritualDamage;
    public void NotifyWeakPointHitBySpiritual()
    {
        weakPointHitBySpiritualDamage = true;
    }


    void Awake()
    {
        transform.rotation = Quaternion.identity;

        if (baseEnemyAttributes != null)
        {
            enemyAttributes = Instantiate(baseEnemyAttributes);

            int maxHp = Mathf.Max(1, enemyAttributes.maxHitPoints);
            health = maxHp;
            speed = enemyAttributes.speed;
            rb=GetComponent<Rigidbody2D>();
        }
        else
        {
            health = Mathf.Max(1, health);
        }
        default_enemy_state = new Enemy_Wander(wanderTime, wanderRadius);
    }



    public override void Attack(Transform target)
    {
        
        StartCoroutine(DashAttack());
    }





    private IEnumerator DashAttack()
    {
        EnableAttack();

        Vector2 dashDirection = transform.up;
        float elapsed = 0f;

        if (rb != null && agent != null)
        {
            agent.canMove = false;
            while (elapsed < dashDuration)
            {
                if (attackHitConnected) break;
                rb.linearVelocity = dashDirection * dashSpeed;
                elapsed += Time.deltaTime;
                yield return null;
            }
            rb.linearVelocity = Vector2.zero;
            agent.canMove = true;
        }

        // Disable hitbox after dash
        DisableAttack();


    }


    public override void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        if (dmgType == DamageInstance.DamageType.Spiritual && weakPointHitBySpiritualDamage)
        {
            weakPointHitBySpiritualDamage = false;
            if (enemyEffectManager != null)
            {
                enemyEffectManager.AddEffect(new Weak_Effect(3f, 0.5f), enemyAttributes);
                enemyEffectManager.AddEffect(new Paralyze_Effect(3f), enemyAttributes);
            }
        }
        base.TakeDamage(amount, dmgSource, dmgType);
    }
}
