using UnityEngine;

public class BreakableObstacle : MonoBehaviour
{
    [Header("Break Settings")]
    [Tooltip("Disable instead of destroy (for pooling or effects)")]
    public bool disableInsteadOfDestroy = false;

    private bool isBroken = false;

    /// <summary>
    /// Call this when the obstacle is hit by an attack.
    /// </summary>
    public void Break()
    {
        if (isBroken) return;

        isBroken = true;

        // play animation / sound here

        if (disableInsteadOfDestroy)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // collision attacks
    private void OnTriggerEnter2D(Collider2D other)
    {
        // projectile hits wall
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            Break();
            Destroy(projectile.gameObject);
        }
    }
}
