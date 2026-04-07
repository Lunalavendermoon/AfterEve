using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] int damage = 40;
    [SerializeField] float radius = 1.2f;
    [SerializeField] LayerMask playerLayers;
    [SerializeField] float destroyAfter = 0.35f;
    void Start()
    {
        
        var hit = Physics2D.OverlapCircle(transform.position, radius, playerLayers);
        if (hit != null && hit.CompareTag("Player") && PlayerController.instance != null)
        {
            PlayerController.instance.TakeDamage(
                damage,
                DamageInstance.DamageSource.Enemy,
                DamageInstance.DamageType.Physical);
        }
        Destroy(gameObject, destroyAfter);
    }

}
