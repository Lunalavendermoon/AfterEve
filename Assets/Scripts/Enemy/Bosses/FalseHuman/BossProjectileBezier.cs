using UnityEngine;

public class BossProjectileBezier: MonoBehaviour
{
    private GameObject target;
    private const float DistanceToTargetToDestroy = 0.5f;
    // Cubic Bezier: B(t) = (1-t)³P0 + 3(1-t)²t P1 + 3(1-t)t² P2 + t³ P3
    private Vector3 p0, p1, p2, p3;
    private float t;
    private float curveLength;
    private float moveSpeed;
    private Vector3 projectileMoveDir;
    private const int SpiritualDamageAmount = 30;

    private const float PlayerHitRadius = 0.5f;
    private bool hasDealtDamage;
    /// <summary>
    /// Initialize: follow a cubic Bezier from current position (spawn) to target position.
    /// trajectoryMaxHeight scales the random arc bulge (e.g. 0.5–1).
    /// </summary>
    public void InitializeProjectile(GameObject target, float maxMoveSpeed, float trajectoryMaxHeight)
    {
        this.target = target;
        p0 = transform.position;
        p3 = target.transform.position;
        Vector2 p0xy = p0;
        Vector2 p3xy = p3;
        Vector2 toTarget = p3xy - p0xy;
        float dist = toTarget.magnitude;
        if (dist < 0.001f)
        {
            curveLength = 0f;
            moveSpeed = maxMoveSpeed;
            return;
        }
        Vector2 dir = toTarget / dist;
        Vector2 perp = new Vector2(-dir.y, dir.x);
        float arcMagnitude = Mathf.Max(dist * Mathf.Clamp01(trajectoryMaxHeight) + 0.5f, 0.5f);
        // Random control points: ~1/3 along the line ± perpendicular bulge for varied arcs
        p1 = p0 + (Vector3)(0.33f * toTarget + (Random.Range(-1f, 1f) * arcMagnitude) * perp);
        p1.z = p0.z;
        p2 = p3 + (Vector3)(0.33f * (p0xy - p3xy) + (Random.Range(-1f, 1f) * arcMagnitude) * perp);
        p2.z = p3.z;
        curveLength = ApproximateBezierLength();
        moveSpeed = curveLength > 0.001f ? maxMoveSpeed : 0f;
        t = 0f;
    }
    private static Vector3 CubicBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        float u = 1f - t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t2 = t * t;
        float t3 = t2 * t;
        return u3 * a + 3f * u2 * t * b + 3f * u * t2 * c + t3 * d;
    }
    private float ApproximateBezierLength()
    {
        const int samples = 16;
        float len = 0f;
        Vector3 prev = CubicBezier(p0, p1, p2, p3, 0f);
        for (int i = 1; i <= samples; i++)
        {
            float s = i / (float)samples;
            Vector3 next = CubicBezier(p0, p1, p2, p3, s);
            len += Vector3.Distance(prev, next);
            prev = next;
        }
        return len;
    }
    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        if (curveLength < 0.001f)
        {
            transform.position = p3;
            OnReachTarget();
            return;
        }
        t += (moveSpeed * Time.deltaTime) / curveLength;
        if (t >= 1f)
        {
            t = 1f;
            transform.position = p3;
            OnReachTarget();
            return;
        }
        Vector3 newPosition = CubicBezier(p0, p1, p2, p3, t);
        projectileMoveDir = (newPosition - transform.position).normalized;
        transform.position = newPosition;

        if (!hasDealtDamage && PlayerController.instance != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            if (distToPlayer <= PlayerHitRadius)
            {
                hasDealtDamage = true;
                PlayerController.instance.TakeDamage(SpiritualDamageAmount, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Spiritual);
                if (target != null) Destroy(target);
                Destroy(gameObject);
                return;
            }
        }

    }
    private void OnReachTarget()
    {
        //if (PlayerController.instance != null)
        //    PlayerController.instance.TakeDamage(SpiritualDamageAmount, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Spiritual);
        if (target != null) Destroy(target);
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (target != null)
            Destroy(target);
    }
    public Vector3 GetProjectileMoveDir() => projectileMoveDir;
}
