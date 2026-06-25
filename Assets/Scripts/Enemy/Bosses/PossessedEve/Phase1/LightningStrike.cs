using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] int damage = 40;
    [SerializeField] float radius = 1.2f;
    [SerializeField] LayerMask playerLayers;
    [SerializeField] float destroyAfter = 0.35f;

    void Start()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, playerLayers);

        if (hits != null)
        {
            foreach (Collider2D hit in hits)
            {
                PlayerController player = hit.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(
                        damage,
                        DamageInstance.DamageSource.Enemy,
                        DamageInstance.DamageType.Physical);
                    break;
                }
            }
        }

        Destroy(gameObject, destroyAfter);
    }
}
