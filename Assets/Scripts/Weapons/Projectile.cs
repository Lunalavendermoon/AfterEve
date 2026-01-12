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

    private int projectileDamage;

    public void SetProjectileDamage(int n)
    {
        projectileDamage = n;
    }
    // new
    public void SetBulletBounce(int n)
    {
        bulletBounces = n;
    }
    public void SetBulletPiercing(int n) { bulletPiercing = n; }
    public void SetBulletEffect(Effects bulletEffect)

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
            for (int i = 0; i < PlayerController.instance.playerAttributes.enemiesChained; i++)
            {
                GameObject temp = FindNearestUnchainedEnemy(other.gameObject);
                if(temp != null)
                {
                    temp.GetComponent<EnemyBase>().Chain(PlayerController.instance.playerAttributes.chainTime);
                    if(PlayerController.instance.IsInSpiritualVision())
                    {
                        PlayerController.instance.health += (int) (PlayerController.instance.playerAttributes.shield * PlayerController.instance.playerAttributes.chainShieldIncrease * 2);
                    } else
                    {
                        PlayerController.instance.health += (int)(PlayerController.instance.playerAttributes.shield * PlayerController.instance.playerAttributes.chainShieldIncrease);
                    }
                }
            }
            DamageAllChainedEnemies();
            // TODO change to DamageType.Spiritual if player is dealing spiritual damage
            enemy.TakeDamage(projectileDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Basic);
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
        //Debug.Log("collision");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            //Debug.Log("collision detected");
            if (bulletBounces > 0)
            {
                //Debug.Log("redirecting");
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

    GameObject FindNearestUnchainedEnemy(GameObject justHitEnemy)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= PlayerController.instance.playerAttributes.chainRadius && !enemy.GetComponent<EnemyBase>().IsChained() && enemy != justHitEnemy)
            {
                closestDistance = distance;
                nearest = enemy;
            }
        }
        return nearest;
    }

    void DamageAllChainedEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyBase>().IsChained())
            {
                enemy.GetComponent<EnemyBase>().TakeDamage((int)(projectileDamage*PlayerController.instance.playerAttributes.chainDmg), DamageInstance.DamageSource.Player, DamageInstance.DamageType.Basic);
            }
        }
    }
}