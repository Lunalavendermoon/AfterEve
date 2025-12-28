using UnityEngine;

public class FreezeObstacle : MonoBehaviour
{
    [SerializeField] private ObstacleData data;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity == null || data == null) return;

        ApplyFreeze(entity);
    }

    private void ApplyFreeze(Entity entity)
    {
        EntityAttributes targetAttr = entity.Attributes;
        if (targetAttr == null) return;

        float duration = data.CalculateFreezeDuration(targetAttr);
        if (duration <= 0f) return;

        entity.ApplyFreeze(duration);
    }
}
