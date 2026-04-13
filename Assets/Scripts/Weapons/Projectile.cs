using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite pierceSprite;
    public Sprite bounceSprite;

    private Vector3 firingPoint;
    private Rigidbody2D rb;
    private Collider2D col;

    [SerializeField] private float projectileSpeed;
    [SerializeField] private float maxProjectileDistance;

    public static event Action<EnemyBase> OnEnemyHit;
    public static event Action<EnemyBase, int, int> OnEnemyHitWithDamage;

    private int bulletBounces;
    private List<Effects> bulletEffects;
    private int bulletPiercing;
    private int enemiesHit;

    private int physicalDamage;
    private int spiritualDamage;

    // After using up all bounces, destroy the bullet on the NEXT hit that should end it.
    private bool bounceQueueDestroyBullet = false;

    private Vector2 moveDir;

    public int SpiritualDamage => spiritualDamage;

    public void SetPhysicalDamage(int n) => physicalDamage = n;
    public void SetSpiritualDamage(int n) => spiritualDamage = n;
    public void AddPhysicalDamage(int n) => physicalDamage += n;
    public void AddSpiritualDamage(int n) => spiritualDamage += n;

    public void SetBulletBounce(int n)
    {
        bulletBounces = n;

        if (n > 0 && spriteRenderer != null)
            spriteRenderer.sprite = bounceSprite;

        ApplyBounceCollisionMode();
    }

    public void SetBulletPiercing(int n)
    {
        bulletPiercing = n;

        if (n > 0 && spriteRenderer != null)
            spriteRenderer.sprite = pierceSprite;
    }

    public void SetBulletEffects(List<Effects> newBulletEffects)
    {
        bulletEffects = newBulletEffects;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.freezeRotation = true;
        }
    }

    private void Start()
    {
        firingPoint = transform.position;
        enemiesHit = 0;

        // Infer initial travel direction from the projectile's current visual forward projected into XY.
        Vector2 initialDir = new Vector2(transform.forward.x, transform.forward.y);
        if (initialDir.sqrMagnitude < 0.0001f)
            initialDir = Vector2.right;

        moveDir = initialDir.normalized;

        ApplyBounceCollisionMode();
        ApplyVelocity();
    }

    private void FixedUpdate()
    {
        ApplyVelocity();
    }

    private void Update()
    {
        if (Vector3.Distance(firingPoint, transform.position) > maxProjectileDistance)
            Destroy(gameObject);
    }

    private void ApplyVelocity()
    {
        if (rb == null)
            return;

        rb.linearVelocity = moveDir * projectileSpeed;
    }

    private void ApplyBounceCollisionMode()
    {
        if (col == null)
            return;

        // Use trigger while not bouncing; use collision while bouncing so we can read contact normals.
        col.isTrigger = bulletBounces <= 0;
        Debug.Log($"[Bullet] ApplyBounceCollisionMode -> isTrigger={col.isTrigger}, bulletBounces={bulletBounces}");
    }

    private IEnumerator ApplyBounceCollisionModeNextFixedStep()
    {
        yield return new WaitForFixedUpdate();
        ApplyBounceCollisionMode();
    }

    private void ApplyVisualRotationFromMoveDir()
    {
        if (moveDir.sqrMagnitude < 0.0001f)
            return;

        float pitchX = -Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(pitchX, 90f, 0f);
    }

    private void HandleBulletHit(GameObject other)
    {
        if (other == null)
            return;

        if (other.GetComponent<PlayerController>())
        {
            return;
        }
        else if (other.TryGetComponent(out EnemyBase enemy))
        {
            if (PlayerController.instance.playerAttributes.hermitPast)
            {
                float dist = Vector3.Distance(transform.position, firingPoint);
                AddPhysicalDamage((int)(PlayerController.instance.playerAttributes.damage * dist * Hermit_Past.dmgBonusPerUnit));
            }

            OnEnemyHit?.Invoke(enemy);
            OnEnemyHitWithDamage?.Invoke(enemy, physicalDamage, spiritualDamage);

            if (physicalDamage > 0)
                enemy.TakeDamage(physicalDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Physical);

            if (spiritualDamage > 0)
            {
                Debug.Log("Spiritual Damage Dealt");
                enemy.TakeDamage(spiritualDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Spiritual);
            }

            if (enemy.TryGetComponent(out EffectManager effectManager) && bulletEffects != null)
            {
                foreach (Effects bulletEffect in bulletEffects)
                    effectManager.AddEffect(bulletEffect, enemy.enemyAttributes);
            }

            enemiesHit++;

            // If bounce mode is active, enemy hit should NOT auto-destroy unless piercing limit was exceeded and you want to queue it.
            if (bulletPiercing > 0)
            {
                if (enemiesHit > bulletPiercing)
                {
                    if (bulletBounces > 0)
                        bounceQueueDestroyBullet = true;
                    else
                        Destroy(gameObject);
                }
            }
            else
            {
                // No piercing. Destroy only when bounce mode is already gone.
                if (bulletBounces <= 0)
                    Destroy(gameObject);
            }
        }
        else
        {
            if (bounceQueueDestroyBullet)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Bullet] OnTriggerEnter2D fired with {(other == null ? "NULL" : other.name)}, bulletBounces={bulletBounces}");

        if (bulletBounces > 0)
            return;

        if (other == null)
            return;

        int obstacleLayer = LayerMask.NameToLayer("Obstacle");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        Debug.Log($"[Bullet] Trigger layer = {LayerMask.LayerToName(other.gameObject.layer)}");

        if (other.gameObject.layer == enemyLayer)
        {
            Debug.Log("[Bullet] Trigger enemy hit");
            HandleBulletHit(other.gameObject);
            return;
        }

        if (other.gameObject.layer == obstacleLayer)
        {
            Debug.Log("[Bullet] Trigger obstacle hit -> destroy");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Bullet] OnCollisionEnter2D fired with {(collision == null || collision.collider == null ? "NULL" : collision.collider.name)}");

        if (bulletBounces <= 0)
            return;

        if (collision == null || collision.collider == null)
            return;

        int obstacleLayer = LayerMask.NameToLayer("Obstacle");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        GameObject other = collision.collider.gameObject;

        Debug.Log($"[Bullet] Collision layer={LayerMask.LayerToName(other.layer)}, bulletBounces={bulletBounces}, velocity={rb.linearVelocity}");

        if (other.layer != obstacleLayer && other.layer != enemyLayer)
        {
            Debug.Log("[Bullet] Collision ignored because layer is not Obstacle or Enemy");
            return;
        }

        Vector2 surfaceNormal = collision.GetContact(0).normal;
        if (surfaceNormal.sqrMagnitude < 0.0001f)
        {
            surfaceNormal = ((Vector2)transform.position - (Vector2)other.transform.position).normalized;
            Debug.Log($"[Bullet] Fallback NORMAL = {surfaceNormal}");
        }

        Vector2 inDir2 = moveDir.normalized;
        Vector2 outDir2 = Vector2.Reflect(inDir2, surfaceNormal).normalized;

        Debug.Log($"[Bullet] IN={inDir2}, NORMAL={surfaceNormal}, OUT={outDir2}");

        // Push slightly out so we don't remain embedded in the collider.
        rb.position += surfaceNormal * 0.02f;

        moveDir = outDir2;
        ApplyVelocity();
        ApplyVisualRotationFromMoveDir();

        bulletBounces--;
        Debug.Log($"[Bullet] bulletBounces after hit = {bulletBounces}");

        // Enemy should still take damage while bouncing.
        HandleBulletHit(other);

        // After the last bounce is consumed, switch modes on the next fixed step, not inside this collision callback.
        if (bulletBounces <= 0)
        {
            Debug.Log("[Bullet] Scheduling bounce collision mode switch");
            StartCoroutine(ApplyBounceCollisionModeNextFixedStep());
        }
    }
}