using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SpiritualDamageWall : MonoBehaviour
{
    [Header("Damage Settings")]
    public bool dealsDamage = true;
    public int baseDamage = 5;

    private BoxCollider2D wallCollider;

    private void Awake()
    {
        wallCollider = GetComponent<BoxCollider2D>();
        wallCollider.isTrigger = false; // acts as wall by default
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // palyer
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            HandlePlayerCollision(player);
            return;
        }

        //eneym
        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
        if (enemy != null && dealsDamage)
        {
            enemy.TakeDamage(baseDamage, DamageInstance.DamageSource.Environment, DamageInstance.DamageType.Spiritual);
        }
    }

    private void HandlePlayerCollision(PlayerController player)
    {
        PlayerAttributes attrs = player.playerAttributes;

        // IGNORREEEEEE
        if (attrs != null && attrs.isEnlightened)
        {
            // disable collider so spiritiual player passes through
            wallCollider.enabled = false;

            // renable for enemies and future collisions
            StartCoroutine(ReenableColliderNextFrame());
            return;
        }

        // damage player if true
        if (dealsDamage && !attrs.isEnlightened)
        {
            player.TakeDamage(baseDamage, DamageInstance.DamageSource.Environment, DamageInstance.DamageType.Spiritual);
        }
    }

    private System.Collections.IEnumerator ReenableColliderNextFrame()
    {
        yield return null;
        wallCollider.enabled = true;
    }
}
