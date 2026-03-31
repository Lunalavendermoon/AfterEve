using UnityEngine;

public class HighKnightGroundMark : MonoBehaviour
{
    private int damagePerTick = 10;
    private float tickInterval = 1f;
    private float tickAccumulator;
    private BoxCollider2D box;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Configure(float length, float width, int damageEachTick, float tickSeconds, float lifetimeSeconds)
    {
        damagePerTick = damageEachTick;
        tickInterval = Mathf.Max(0.05f, tickSeconds);
        if (box == null)
            box = GetComponent<BoxCollider2D>();
        transform.localScale = new Vector3(length, width, 1f);
        box.size = Vector2.one;
        if (spriteRenderer != null)
        {
            spriteRenderer.drawMode = SpriteDrawMode.Simple;
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }
        Destroy(gameObject, lifetimeSeconds);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || PlayerController.instance == null)
            return;
        tickAccumulator += Time.deltaTime;
        while (tickAccumulator >= tickInterval)
        {
            tickAccumulator -= tickInterval;
            PlayerController.instance.TakeDamage(
                damagePerTick,
                DamageInstance.DamageSource.Enemy,
                DamageInstance.DamageType.Physical);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            tickAccumulator = 0f;
    }
}

