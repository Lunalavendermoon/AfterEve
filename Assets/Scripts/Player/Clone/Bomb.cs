using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    int baseDamage;
    int spiritualDamage;
    float strengthBuff;
    Vector3 firingPoint;
    bool moving = true;
    public float maxProjectileDistance;
    public float timeTillExplosion;
    float timeStopped;
    bool exploding = false;
    public float explosionTime;
    public float speed;
    public static event Action<EnemyBase> OnEnemyHit;

    void Start()
    {
        firingPoint = transform.position;
    }

    void Update()
    {
        MoveBomb();
    }

    void MoveBomb()
    {
        if (moving)
        {
            if (Vector3.Distance(firingPoint, transform.position) > maxProjectileDistance)
            {
                moving = false;
                timeStopped = Time.time;
            }
            else
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        } else
        {
            if (!exploding)
            {
                if (Time.time - timeStopped >= timeTillExplosion)
                {
                    exploding = true;
                    timeStopped = Time.time;
                    transform.localScale *= 3f;
                }
            } else
            {
                if (Time.time - timeStopped >= explosionTime)
                {
                    Destroy(gameObject);
                }
            }
            
        }
    }

        

    public void SetBasicDamage(int n)
    {
        baseDamage = n;
    }

    public void SetSpiritualDamage(int n)
    {
        spiritualDamage = n;
    }

    public void SetStrengthBuff(float f)
    {
        strengthBuff = f;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyBase>() && exploding)
        {
            EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
            OnEnemyHit?.Invoke(enemy);
            enemy.TakeDamage(baseDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Basic);
            enemy.TakeDamage(spiritualDamage, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Spiritual);
            enemy.Mark(strengthBuff);
        }
    }

}
