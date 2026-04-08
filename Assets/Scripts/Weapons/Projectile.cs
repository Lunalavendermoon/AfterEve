using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite pierceSprite;
    public Sprite bounceSprite;
    private Vector3 firingPoint;

    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private float maxProjectileDistance;

    public static event Action<EnemyBase> OnEnemyHit;
    public static event Action<EnemyBase, int, int> OnEnemyHitWithDamage;
    private int bulletBounces;
    private List<Effects> bulletEffects;
    private int bulletPiercing;
    private int enemiesHit;

    private int physicalDamage;
    private int spiritualDamage;

    // After using up all bounces, destroy the bullet on the NEXT enemy hit
    private bool bounceQueueDestroyBullet = false;
    public int SpiritualDamage => spiritualDamage;
    public void SetPhysicalDamage(int n)
    {
        physicalDamage = n;
    }
    public void SetSpiritualDamage(int n)
    {
        spiritualDamage = n;
    }
    public void AddPhysicalDamage(int n)
    {
        physicalDamage += n;
    }
    public void AddSpiritualDamage(int n)
    {
        spiritualDamage += n;
    }
    // new
    public void SetBulletBounce(int n)
    {
        bulletBounces = n;
        if (n > 0)
        {
            spriteRenderer.sprite = bounceSprite;
        }
    }
    public void SetBulletPiercing(int n) {
        bulletPiercing = n;
        // TODO dmg reduction after each pierce/bounce
        if (n > 0)
        {
            spriteRenderer.sprite = pierceSprite;
        }
    }

    public void SetBulletEffects(List<Effects> bulletEffects)

    {
        this.bulletEffects = bulletEffects;
    }

    void Start()
    {
        firingPoint = transform.position;
        enemiesHit = 0;
    }

    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
    }

    void MoveProjectile()
    {
        if (Vector3.Distance(firingPoint, transform.position) > maxProjectileDistance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }
    }

    void HandleBulletHit(GameObject other)
    {
        if (other.GetComponent<PlayerController>())
        {
        }
        else if (other.GetComponent<EnemyBase>())
        {
            if (PlayerController.instance.playerAttributes.hermitPast)
            {
                float dist = Vector3.Distance(transform.position, firingPoint);
                AddPhysicalDamage((int)(PlayerController.instance.playerAttributes.damage * dist * Hermit_Past.dmgBonusPerUnit));
            }

            EnemyBase enemy = other.GetComponent<EnemyBase>();
            OnEnemyHit?.Invoke(enemy);
            OnEnemyHitWithDamage?.Invoke(enemy, physicalDamage, spiritualDamage);

            if (physicalDamage > 0)
            {
                enemy.TakeDamage(physicalDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Physical);
            }
            if (spiritualDamage > 0)
            {
                Debug.Log("Spiritual Damage Dealt");
                enemy.TakeDamage(spiritualDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Spiritual);
            }

            if (enemy.GetComponent<EffectManager>())
            {
                if (!(bulletEffects == null))
                    foreach(Effects bulletEffect in bulletEffects)
                        enemy.GetComponent<EffectManager>().AddEffect(bulletEffect, enemy.enemyAttributes);
            }
            enemiesHit++;
            if ((bulletPiercing > 0 && enemiesHit > bulletPiercing) || (bulletPiercing == 0 && bounceQueueDestroyBullet))
            {
                Destroy(gameObject);
            }
        }
        //else if (other.GetComponent<BossBehaviourBase>())
        //{
        //    BossBehaviourBase boss = other.GetComponent<BossBehaviourBase>();
        //    if (physicalDamage > 0)
        //        boss.TakeDamage(physicalDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Physical);
        //    if (spiritualDamage > 0)
        //        boss.TakeDamage(spiritualDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Spiritual);
        //    enemiesHit++;
        //    if ((bulletPiercing > 0 && enemiesHit > bulletPiercing) || (bulletPiercing == 0 && bounceQueueDestroyBullet))
        //        Destroy(this.gameObject);
        //}
        else
        {
            if (bounceQueueDestroyBullet)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("collision");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") || collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            //Debug.Log("collision detected");
            if (bulletBounces > 0)
            {
                //Debug.Log("redirecting");
                Vector3 surfaceNormal = collision.contacts[0].normal;
                Vector3 reflectedDirection = Vector3.Reflect(transform.forward, surfaceNormal);
                transform.forward = reflectedDirection.normalized;
                bulletBounces--;
            } else
            {
                bounceQueueDestroyBullet = true;
            }
            HandleBulletHit(collision.gameObject);
        }
    }
}