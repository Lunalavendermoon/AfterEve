using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class Projectile : MonoBehaviour
{
    private Vector3 firingPoint;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private float maxProjectileDistance;
    public static event Action<EnemyBase> OnEnemyHit;
    private int bulletBounces;
    private Effects bulletEffect;
    private int bulletPiercing;
    private int enemiesPierced;
    public void setBulletBounce(int n)
    {
        bulletBounces = n;
    }
    public void setBulletPiercing(int n) { bulletPiercing = n; }
    public void setBulletEffect(Effects bulletEffect)
    {
        this.bulletEffect = bulletEffect;
    }
    void Start()
    {
        firingPoint = transform.position;
        enemiesPierced = 0;
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
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
        }
        else if (other.gameObject.GetComponent<EnemyBase>())
        {
            EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
            OnEnemyHit?.Invoke(enemy);
            // TODO change to DamageType.Spiritual if player is dealing spiritual damage
            enemy.TakeDamage(PlayerController.instance.playerAttributes.damage,
                DamageInstance.DamageSource.Player, DamageInstance.DamageType.Basic);
            if (enemy.GetComponent<EffectManager>())
            {
                if (!(bulletEffect == null))
                    enemy.GetComponent<EffectManager>().AddEffect(bulletEffect);
            }
            enemiesPierced++;
            if (enemiesPierced == bulletPiercing)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Debug.Log("collision detected");
            if (bulletBounces > 0)
            {
                Debug.Log("redirecting");
                Vector3 surfaceNormal = collision.contacts[0].normal;
                Vector3 reflectedDirection = Vector3.Reflect(transform.forward, surfaceNormal);
                transform.forward = reflectedDirection.normalized;
                bulletBounces--;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}