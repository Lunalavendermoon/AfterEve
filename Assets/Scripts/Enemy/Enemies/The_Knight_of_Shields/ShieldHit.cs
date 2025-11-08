using UnityEngine;

public class ShieldHit : MonoBehaviour
{
   public void OnTriggerEnter(Collider other)
    {
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            Debug.Log("Shield hit by projectile!");
            Destroy(other.gameObject); // Destroy the projectile on shield hit
        }
    }
}
