using UnityEngine;

public class HighKnightRangedProjectile : MonoBehaviour
{
    private HighKnight owner;
    private Vector3 knightShotPosition;
    private Vector2 direction;
    private float speed;
    private float maxTravelDistance;
    private float traveled;
    public void Initialize(HighKnight knight, Vector3 shotOrigin, Vector2 dir, float moveSpeed, float maxDistance)
    {
        owner = knight;
        knightShotPosition = shotOrigin;
        direction = dir.normalized;
        speed = moveSpeed;
        maxTravelDistance = maxDistance;
        traveled = 0f;
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

    }
    private void Update()
    {
        float step = speed * Time.deltaTime;
        traveled += step;
        transform.position += (Vector3)(direction * step);
        if (traveled >= maxTravelDistance)
            Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        owner.SpawnGroundMarkBetween(knightShotPosition, transform.position);
        if (!Application.isPlaying || owner == null)
            return;
        
    }
}
