using UnityEngine;
using System.Collections;
public class KnightofHammersScript : StandardEnemyBase, IKnightWithWeakPoint
{
    [SerializeField] private float wanderRadius = 7.5f;
    [SerializeField] private float wanderTime = 3f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int jumpCount = 3;
    [SerializeField] private float jumpDistance = 3f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private float hitRadius = 1f;
    [SerializeField] private float jumpHeight = 1.5f;
    void Awake()
    {
        transform.rotation = Quaternion.identity;

        if (baseEnemyAttributes != null)
        {
            enemyAttributes = Instantiate(baseEnemyAttributes);

            int maxHp = Mathf.Max(1, enemyAttributes.maxHitPoints);
            health = maxHp;
            speed = enemyAttributes.speed;
            rb = GetComponent<Rigidbody2D>();
        }
        else
        {
            health = Mathf.Max(1, health);
        }


        if (playerLayer.value == 0)
            playerLayer = LayerMask.GetMask("Player");
        default_enemy_state = new Enemy_Wander(wanderTime, wanderRadius);

    }

    private bool weakPointHitBySpiritualDamage;
    public void NotifyWeakPointHitBySpiritual()
    {
        weakPointHitBySpiritualDamage = true;
    }

    public override void Attack(Transform target)
    {

        StartCoroutine(JumpAttack(target));
        
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


    private IEnumerator JumpAttack(Transform target)
    {
        isAttacking = true;
        if (agent != null)
        {
            agent.canMove = false;
            agent.isStopped = true;
        }
        DisableAttack();
        for (int i = 0; i < jumpCount; i++)
        {
            Vector3 directionToPlayer = target.position - transform.position;
            float distance = directionToPlayer.magnitude;
            Vector3 jumpDirection = distance > 0.001f ? directionToPlayer.normalized : Vector3.right;
            float distanceThisJump = Mathf.Min(distance, jumpDistance);
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + jumpDirection * distanceThisJump;
            float elapsed = 0f;
            while (elapsed < jumpDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / jumpDuration);
                float heightOffset = 4f * jumpHeight * t * (1f - t);
                transform.position = Vector3.Lerp(startPos, targetPos, t) - Vector3.forward * heightOffset;
                yield return null;
            }
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, playerLayer);
            foreach (Collider2D hit in hits)
            {
                if (!hit.CompareTag("Player")) continue;
                int damage = enemyAttributes.damage;
                PlayerController.instance.TakeDamage(damage, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Physical);
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        if (agent != null)
        {
            agent.isStopped = false;
            agent.canMove = true;
        }
        DisableAttack();
        isAttacking = false;
    }
}


