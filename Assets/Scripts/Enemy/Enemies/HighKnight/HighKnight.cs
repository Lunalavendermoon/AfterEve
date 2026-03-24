using UnityEngine;

public class HighKnight : StandardEnemyBase, IKnightWithWeakPoint
{
    [Header("Wander")]
    [SerializeField] private float wanderRadius = 7.5f;
    [SerializeField] private float wanderTime = 3f;
    [Header("Combat")]
    [SerializeField] private float meleeModeMaxDistance = 7f;
    [SerializeField] private float meleeAttackCooldownSeconds = 0.5f;
    [SerializeField] private float rangedAttackCooldownSeconds = 5f;
    [SerializeField] private GameObject rangedProjectilePrefab;
    [Header("Ground mark (after projectile despawn)")]
    [SerializeField] private GameObject groundMarkPrefab;
    [SerializeField] private float markLifetimeAfterProjectileDespawn = 3f;
    [SerializeField] private float rangedMarkWidth = 1f;
    [SerializeField] private int rangedMarkDamagePerTick = 10;
    [SerializeField] private float rangedMarkDamageTickInterval = 1f;
    [Header("Ranged projectile motion")]
    [SerializeField] private float rangedProjectileSpeed = 12f;
    [SerializeField] private float rangedProjectileMaxDistance = 30f;
    private bool combatActive;
    private float rangedCooldownTimer;
    private float meleeCooldownTimer;
    private bool weakPointHitBySpiritualDamage;
    private sealed class NoopCombatState : IEnemyStates
    {
        public void EnterState(StandardEnemyBase enemy) { }
        public void UpdateState(StandardEnemyBase enemy) { }
        public void ExitState(StandardEnemyBase enemy) { }
    }
    private static readonly NoopCombatState NoopCombat = new();
    public void NotifyWeakPointHitBySpiritual()
    {
        weakPointHitBySpiritualDamage = true;
    }
    public override void ChangeState(IEnemyStates newState)
    {
        if (newState is Enemy_Chase)
        {
            combatActive = true;
            rangedCooldownTimer = 0f;
            meleeCooldownTimer = 0f;
            base.ChangeState(NoopCombat);
            return;
        }
        base.ChangeState(newState);
    }
    private void Awake()
    {
        transform.rotation = Quaternion.identity;
        elite = true;
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
        Invoke(nameof(DisableAttack), 0.35f);
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
    public override void EnemyUpdate()
    {
        if (PlayerController.instance == null || enemyAttributes == null)
            return;
        if (enemyAttributes.isParalyzed)
        {
            if (agent != null)
                agent.isStopped = true;
            return;
        }
        if (!InRange(PlayerController.instance.transform))
        {
            if (combatActive)
            {
                combatActive = false;
                ChangeState(new Enemy_Wander(wanderTime, wanderRadius));
            }
            return;
        }
        if (!combatActive)
            return;
        Transform player = PlayerController.instance.transform;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > meleeModeMaxDistance)
        {
            if (agent != null)
            {
                agent.isStopped = true;
                agent.canMove = false;
            }
            if (destinationSetter != null)
                destinationSetter.target = null;
            rangedCooldownTimer -= Time.deltaTime;
            if (rangedCooldownTimer <= 0f)
            {
                FireRangedProjectile();
                rangedCooldownTimer = rangedAttackCooldownSeconds;
            }
        }
        else
        {
            if (agent != null)
            {
                agent.isStopped = false;
                agent.canMove = true;
            }
            Pathfinding(player);
            meleeCooldownTimer -= Time.deltaTime;
            if (InAttackRange(player) && meleeCooldownTimer <= 0f && !isAttacking)
            {
                Attack(player);
                meleeCooldownTimer = meleeAttackCooldownSeconds;
            }
        }
    }
    private void FireRangedProjectile()
    {
        if (rangedProjectilePrefab == null || PlayerController.instance == null)
            return;
        Vector3 origin = transform.position;
        Vector2 dir = ((Vector2)PlayerController.instance.transform.position - (Vector2)origin);
        if (dir.sqrMagnitude < 1e-4f)
            dir = Vector2.right;
        dir.Normalize();
        GameObject proj = Instantiate(rangedProjectilePrefab, origin, Quaternion.identity);
        var hp = proj.GetComponent<HighKnightRangedProjectile>();
        if (hp == null)
            hp = proj.AddComponent<HighKnightRangedProjectile>();
        hp.Initialize(this, origin, dir, rangedProjectileSpeed, rangedProjectileMaxDistance);
    }
    public void SpawnGroundMarkBetween(Vector3 knightShotPosition, Vector3 projectileEndPosition)
    {
        if (groundMarkPrefab == null)
            return;
        Vector2 a = knightShotPosition;
        Vector2 b = projectileEndPosition;
        Vector2 delta = b - a;
        float length = delta.magnitude;
        if (length < 0.01f)
            length = 0.01f;
        Vector2 dir = delta / length;
        Vector2 center = (a + b) * 0.5f;
        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angleDeg);
        GameObject instance = Instantiate(groundMarkPrefab, new Vector3(center.x, center.y, knightShotPosition.z), rot);
        if (!instance.TryGetComponent(out HighKnightGroundMark mark))
            mark = instance.AddComponent<HighKnightGroundMark>();
        mark.Configure(
            length,
            rangedMarkWidth,
            rangedMarkDamagePerTick,
            rangedMarkDamageTickInterval,
            markLifetimeAfterProjectileDespawn);
    }
}
