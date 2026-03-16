using UnityEngine;

/// <summary>
/// Dynamically sets sortingOrder based on Y position, creating a 2.5D depth effect.
/// Lower Y = rendered on top (closer to viewer). Higher Y = rendered behind.
///
/// For walls (axis-aligned):
///   - Attach to the SpriteRenderer GameObject.
///   - Set collisionParent to the child GameObject that holds the BoxCollider2Ds.
///   - isStatic = true.
///
/// For angled/diagonal walls:
///   - Same as above, but set angleCompensation = true and isStatic = false.
///   - Each frame, finds the BoxCollider2D whose X range covers the player,
///     then interpolates along its bottom edge to get the accurate base Y.
///
/// For the player (Spine):
///   - Attach to the SkeletonAnimation child (the one with MeshRenderer).
///   - Set referenceCollider to the player root's BoxCollider2D.
///   - isStatic = false, angleCompensation = false.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class YSort : MonoBehaviour
{
    [Tooltip("1 unit of Y = this many sorting order levels apart. 100 is usually fine.")]
    [SerializeField] private int precision = 100;

    [Tooltip("For the player: assign the player root's BoxCollider2D. Uses bounds.min.y as feet position.")]
    [SerializeField] private Collider2D referenceCollider;

    [Tooltip("For walls: assign the GameObject that contains the wall's BoxCollider2D(s). " +
             "Scans all BoxCollider2Ds under it to find the one at the player's X position.")]
    [SerializeField] private Transform collisionParent;

    [Tooltip("Y offset applied when neither referenceCollider nor collisionParent is set.")]
    [SerializeField] private float yOffset;

    [Tooltip("Only compute sorting order once at Start. Use for non-angled static walls.")]
    [SerializeField] public bool isStatic;

    [Tooltip("For angled/diagonal walls: interpolate along the bottom edge of the matching collider " +
             "at the player's X position. Requires collisionParent and isStatic = false.")]
    [SerializeField] private bool angleCompensation;

    private Renderer rend;
    private BoxCollider2D[] wallColliders;
    private readonly Vector2[] corners = new Vector2[4];

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        if (collisionParent != null)
            wallColliders = collisionParent.GetComponentsInChildren<BoxCollider2D>();
    }

    private void Start()
    {
        UpdateSortingOrder();
    }

    private void LateUpdate()
    {
        if (!isStatic)
            UpdateSortingOrder();
    }

    public void UpdateSortingOrder()
    {
        rend.sortingOrder = Mathf.RoundToInt(-GetSortY() * precision);
    }

    private float GetSortY()
    {
        // Player path: use BoxCollider2D bounds.min.y as feet position.
        if (referenceCollider != null)
            return referenceCollider.bounds.min.y;

        // Wall path: scan all BoxCollider2Ds under collisionParent.
        if (wallColliders != null && wallColliders.Length > 0)
        {
            float playerX = GetPlayerX();

            if (angleCompensation)
                return GetAngleCompensatedY(playerX);
            else
                return GetLowestYAtX(playerX);
        }

        return transform.position.y + yOffset;
    }

    /// <summary>
    /// Finds the BoxCollider2D whose X range covers playerX and returns its bounds.min.y.
    /// Falls back to the global minimum Y across all colliders.
    /// </summary>
    private float GetLowestYAtX(float playerX)
    {
        float fallback = float.MaxValue;

        foreach (var box in wallColliders)
        {
            Bounds b = box.bounds;
            fallback = Mathf.Min(fallback, b.min.y);

            if (playerX >= b.min.x && playerX <= b.max.x)
                return b.min.y;
        }

        return fallback < float.MaxValue ? fallback : transform.position.y;
    }

    /// <summary>
    /// For angled walls: finds the BoxCollider2D whose X range covers playerX,
    /// then interpolates along its bottom edge to get the Y at exactly playerX.
    /// </summary>
    private float GetAngleCompensatedY(float playerX)
    {
        float fallback = float.MaxValue;

        foreach (var box in wallColliders)
        {
            GetWorldCorners(box, corners);

            float minX = float.MaxValue, maxX = float.MinValue;
            foreach (var c in corners)
            {
                minX = Mathf.Min(minX, c.x);
                maxX = Mathf.Max(maxX, c.x);
                fallback = Mathf.Min(fallback, c.y);
            }

            if (playerX < minX || playerX > maxX) continue;

            // This collider spans the player's X — interpolate its bottom edge.
            float y = GetBottomEdgeYAtX(playerX);
            if (y < float.MaxValue) return y;
        }

        return fallback < float.MaxValue ? fallback : transform.position.y;
    }

    private float GetBottomEdgeYAtX(float worldX)
    {
        float minY = float.MaxValue;

        for (int i = 0; i < 4; i++)
        {
            Vector2 a = corners[i];
            Vector2 b = corners[(i + 1) % 4];

            float edgeMinX = Mathf.Min(a.x, b.x);
            float edgeMaxX = Mathf.Max(a.x, b.x);

            if (worldX < edgeMinX || worldX > edgeMaxX) continue;
            if (Mathf.Abs(b.x - a.x) < 0.0001f) continue;

            float t = (worldX - a.x) / (b.x - a.x);
            float y = Mathf.Lerp(a.y, b.y, t);
            if (y < minY) minY = y;
        }

        return minY;
    }

    private void GetWorldCorners(BoxCollider2D box, Vector2[] result)
    {
        Vector2 center = box.transform.TransformPoint(box.offset);
        float angle = box.transform.eulerAngles.z * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        float hw = box.size.x * Mathf.Abs(box.transform.lossyScale.x) * 0.5f;
        float hh = box.size.y * Mathf.Abs(box.transform.lossyScale.y) * 0.5f;

        result[0] = center + Rotate(new Vector2(-hw, -hh), cos, sin);
        result[1] = center + Rotate(new Vector2( hw, -hh), cos, sin);
        result[2] = center + Rotate(new Vector2( hw,  hh), cos, sin);
        result[3] = center + Rotate(new Vector2(-hw,  hh), cos, sin);
    }

    private static Vector2 Rotate(Vector2 v, float cos, float sin)
        => new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);

    private static float GetPlayerX()
    {
        var player = PlayerController.Player;
        return player != null ? player.transform.position.x : 0f;
    }
}