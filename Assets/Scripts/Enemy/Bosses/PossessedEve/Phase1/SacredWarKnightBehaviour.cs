using UnityEngine;

public class SacredWarKnightBehaviour : MonoBehaviour
{
    [SerializeField] StandardEnemyBase enemy;
    [SerializeField] Rigidbody2D rb;
    int shieldPool;
    float contactDamagePerSecond = 10f;
    float contactTickTimer;
    const float ContactTickInterval = 0.2f;
    void Awake()
    {
        if (enemy == null) enemy = GetComponent<StandardEnemyBase>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        EnemyBase.OnEnemyDamageTaken += OnEnemyDamaged;
    }
    void OnDisable()
    {
        EnemyBase.OnEnemyDamageTaken -= OnEnemyDamaged;
    }
    public void ConfigureSacredWar(int shieldBonus, float damagePerSecondWhenTouchingPlayer)
    {
        contactDamagePerSecond = Mathf.Max(0f, damagePerSecondWhenTouchingPlayer);
        shieldPool = Mathf.Max(0, shieldBonus);
        if (shieldPool > 0 && enemy != null)
            enemy.health += shieldPool;
        if (enemy != null && enemy.agent != null)
        {
            enemy.agent.enabled = false;
            enemy.agent.isStopped = true;
        }
    }
    public void SetWallPosition(Vector3 worldPosition)
    {
        worldPosition.z = transform.position.z;
        if (rb != null)
            rb.MovePosition(worldPosition);
        else
            transform.position = worldPosition;
    }
    void OnEnemyDamaged(DamageInstance dmg, EnemyBase target)
    {
        if (enemy == null || target != enemy || shieldPool <= 0)
            return;
        int absorbed = Mathf.Min(shieldPool, dmg.afterReduction);
        if (absorbed <= 0) return;
        shieldPool -= absorbed;
        enemy.health += absorbed;
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (contactDamagePerSecond <= 0f || PlayerController.instance == null)
            return;
        if (!other.GetComponentInParent<PlayerController>())
            return;
        contactTickTimer += Time.deltaTime;
        while (contactTickTimer >= ContactTickInterval)
        {
            contactTickTimer -= ContactTickInterval;
            int tick = Mathf.Max(1, Mathf.RoundToInt(contactDamagePerSecond * ContactTickInterval));
            PlayerController.instance.TakeDamage(tick, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Physical);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>())
            contactTickTimer = 0f;
    }
}