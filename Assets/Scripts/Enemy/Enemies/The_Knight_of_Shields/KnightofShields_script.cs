using UnityEngine;

public class KnightofShields_script : StandardEnemyBase
{
    [SerializeField] private float wanderRadius = 7.5f;
    [SerializeField] private float wanderTime = 3f;
    [Tooltip("Child in front of the knight (e.g. Shield). Used as the forward direction for the 180° block.")]
    [SerializeField] private Transform shieldFrontReference;

    private bool weakPointHitBySpiritualDamage;
    public void NotifyWeakPointHitBySpiritual()
    {
        weakPointHitBySpiritualDamage = true;
    }
    private void Awake()
    {
        transform.rotation = Quaternion.identity;
        if (shieldFrontReference == null)
        {
            Transform found = transform.Find("Shield");
            if (found != null)
                shieldFrontReference = found;
        }
        if (baseEnemyAttributes != null)
        {
            enemyAttributes = Instantiate(baseEnemyAttributes);
            health = Mathf.Max(1, enemyAttributes.maxHitPoints);
            speed = enemyAttributes.speed;
            rb = GetComponent<Rigidbody2D>();
        }
        else if (enemyAttributes != null)
        {
            health = Mathf.Max(1, enemyAttributes.maxHitPoints);
            speed = enemyAttributes.speed;
        }
        else
        {
            health = Mathf.Max(1, health);
        }
        default_enemy_state = new Enemy_Wander(wanderTime, wanderRadius);
    }


    public override void Attack(Transform target)
    {
        EnableAttack();
        Invoke(nameof(DisableAttack), 0.5f);

    }

    public override void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        if (enemyAttributes == null)
            return;
        if (dmgType == DamageInstance.DamageType.Spiritual && weakPointHitBySpiritualDamage)
        {
            weakPointHitBySpiritualDamage = false;
            if (enemyEffectManager != null)
            {
                enemyEffectManager.AddEffect(new Weak_Effect(3f, 0.5f), enemyAttributes);
                enemyEffectManager.AddEffect(new Paralyze_Effect(3f), enemyAttributes);
            }
        }
        if (!enemyAttributes.isParalyzed
            && dmgSource == DamageInstance.DamageSource.Player
            && IsDamageSourceInShieldedFrontArc())
        {
            return;
        }
        base.TakeDamage(amount, dmgSource, dmgType);
    }
    private bool IsDamageSourceInShieldedFrontArc()
    {
        if (PlayerController.instance == null)
            return false;
        Vector2 forward = GetShieldForwardWorldXY();
        if (forward.sqrMagnitude < 1e-6f)
            forward = Vector2.up;
        forward.Normalize();
        Vector2 toSource = (Vector2)PlayerController.instance.transform.position - (Vector2)transform.position;
        if (toSource.sqrMagnitude < 1e-6f)
            return true;
        toSource.Normalize();
        return Vector2.Dot(forward, toSource) > 0f;
    }
    private Vector2 GetShieldForwardWorldXY()
    {
        if (shieldFrontReference != null)
            return (Vector2)shieldFrontReference.position - (Vector2)transform.position;
        return transform.up;
    }
}
