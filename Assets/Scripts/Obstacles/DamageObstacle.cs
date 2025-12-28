using UnityEngine;

public class DamageObstacle : MonoBehaviour
{
    [SerializeField] private ObstacleData data;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity == null || data == null) return;

        ApplyDamage(entity);
    }

    private void ApplyDamage(Entity entity)
    {
        EntityAttributes targetAttr = entity.Attributes;
        if (targetAttr == null) return;

        int finalDamage = data.CalculateDamageTo(targetAttr);

        if (finalDamage > 0)
        {
            entity.TakeDamage(finalDamage);
        }
    }
}
