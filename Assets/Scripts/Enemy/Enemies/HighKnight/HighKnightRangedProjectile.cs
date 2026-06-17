using UnityEngine;

public class HighKnightRangedProjectile : MonoBehaviour
{
    #region Serialized Fields


    [Header("Ground mark (after projectile despawn)")]
    [SerializeField] private HighKnightGroundMark groundMarkPrefab;
    [SerializeField] private float markLifetimeAfterProjectileDespawn = 3f;
    [SerializeField] private float rangedMarkWidth = 1f;
    [SerializeField] private int rangedMarkDamagePerTick = 10;
    [SerializeField] private float rangedMarkDamageTickInterval = 1f;


    #endregion

    #region Private Fields


    private HighKnightGroundMark _mark;

    //private HighKnight owner;
    private Vector3 knightShotPosition;
    private Vector2 direction;
    private float speed;
    private float maxTravelDistance;
    private float traveled;


    #endregion

    #region Monobehavior Callbacks


    private void Update()
    {
        float step = speed * Time.deltaTime;
        traveled += step;
        transform.position += (Vector3)(direction * step);

        // Mark Update
        Vector2 a = knightShotPosition;
        Vector2 b = transform.position;
        Vector2 delta = b - a;
        float length = Mathf.Max(0.01f, delta.magnitude);
        Vector2 center = (a + b) * 0.5f;

        if (_mark != null)
        {
            _mark.transform.position = new Vector3(center.x, center.y, knightShotPosition.z);
            _mark.Configure(
                length,
                rangedMarkWidth,
                rangedMarkDamagePerTick,
                rangedMarkDamageTickInterval);
        }

        if (traveled >= maxTravelDistance)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_mark != null)
        {
            _mark.StartLifeTime(markLifetimeAfterProjectileDespawn);
            _mark = null;
        }
    }


    #endregion

    #region Public Methods


    public void Initialize(Vector3 shotOrigin, Vector2 dir, float moveSpeed, float maxDistance)
    {
        knightShotPosition = shotOrigin;
        direction = dir.normalized;
        speed = moveSpeed;
        maxTravelDistance = maxDistance;
        traveled = 0f;

        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Projectile Mark Initialization
        if (groundMarkPrefab == null)
            return;

        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angleDeg);
        _mark = Instantiate(groundMarkPrefab, knightShotPosition, rot);
    }

    public void ConfigureMark(HighKnightGroundMark markPrefab, float width, int damageEachTick, float tickSeconds, float lifetimeSeconds)
    {
        groundMarkPrefab = markPrefab;
        markLifetimeAfterProjectileDespawn = lifetimeSeconds;
        rangedMarkWidth = width;
        rangedMarkDamagePerTick = damageEachTick;
        rangedMarkDamageTickInterval = tickSeconds;
    }


    #endregion
}
