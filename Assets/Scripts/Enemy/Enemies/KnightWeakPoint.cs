using UnityEngine;

public class KnightWeakPoint : MonoBehaviour
{
    [SerializeField] private float cooldownAfterHit = 5f;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private bool inCooldown;
    private float cooldownEndTime;
    private bool spiritualVisionOn;
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (col != null) col.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }
    private void OnEnable()
    {
        if (PlayerController.instance != null)
            spiritualVisionOn = PlayerController.instance.IsInSpiritualVision();
        PlayerController.OnSpiritualVisionChange += OnSpiritualVisionChange;
        RefreshVisibility();
    }
    private void OnDisable()
    {
        PlayerController.OnSpiritualVisionChange -= OnSpiritualVisionChange;
    }
    private void Update()
    {
        if (inCooldown && Time.time >= cooldownEndTime)
        {
            inCooldown = false;
            RefreshVisibility();
        }
    }
    private void OnSpiritualVisionChange(bool isOn)
    {
        spiritualVisionOn = isOn;
        RefreshVisibility();
    }
    private void RefreshVisibility()
    {
        bool visible = spiritualVisionOn && !inCooldown;
        if (col != null) col.enabled = visible;
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile == null || projectile.SpiritualDamage <= 0 || !CanBeHit()) return;
        IKnightWithWeakPoint knight = GetComponentInParent<IKnightWithWeakPoint>();
        if (knight != null)
            knight.NotifyWeakPointHitBySpiritual();
        inCooldown = true;
        cooldownEndTime = Time.time + cooldownAfterHit;
        RefreshVisibility();
    }
    public bool CanBeHit()
    {
        return spiritualVisionOn && !inCooldown;
    }
}
