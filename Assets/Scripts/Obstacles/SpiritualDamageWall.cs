using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SpiritualDamageWall : MonoBehaviour
{
    [Header("Damage Settings")]
    public bool dealsDamage = true;
    public int baseDamage = 5;

    [Header("Wall Blocking")]
    [Tooltip("指向实际阻挡玩家的 Collider（如 TilemapCollider 或独立实体墙）")]
    public Collider2D blockingCollider;

    private int enlightenedInsideCount;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            HandlePlayerEnter(player);
            return;
        }

        EnemyBase enemy = other.GetComponentInParent<EnemyBase>();
        if (enemy != null && dealsDamage)
        {
            enemy.TakeDamage(baseDamage, DamageInstance.DamageSource.Environment, DamageInstance.DamageType.Spiritual);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            HandlePlayerExit(player);
        }
    }

    private void HandlePlayerEnter(PlayerController player)
    {
        PlayerAttributes attrs = player.playerAttributes;
        if (attrs == null) return;

        // IGNORREEEEEE
        if (attrs.isEnlightened)
        {
            if (blockingCollider != null)
            {
                enlightenedInsideCount++;
                blockingCollider.enabled = false;
            }
            return;
        }
        
        // damage player if true
        if (dealsDamage)
        {
            player.TakeDamage(baseDamage, DamageInstance.DamageSource.Environment, DamageInstance.DamageType.Spiritual);
        }
    }

    private void HandlePlayerExit(PlayerController player)
    {
        PlayerAttributes attrs = player.playerAttributes;
        if (attrs == null || !attrs.isEnlightened) return;

        if (blockingCollider != null)
        {
            enlightenedInsideCount = Mathf.Max(0, enlightenedInsideCount - 1);
            if (enlightenedInsideCount == 0)
            {
                blockingCollider.enabled = true;
            }
        }
    }
}