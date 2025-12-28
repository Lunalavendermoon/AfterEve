using UnityEngine;

public class TeleportObstacle : MonoBehaviour
{
    [SerializeField] private Transform teleportTarget;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity == null || teleportTarget == null) return;

        Teleport(entity);
    }

    private void Teleport(Entity entity)
    {
        entity.transform.position = teleportTarget.position;
    }
}
