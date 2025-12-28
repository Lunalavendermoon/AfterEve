using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField]
    Transform firingPoint;

    [SerializeField]
    GameObject projectilePrefab;

    private float firingSpeed;

    public static PlayerGun Instance;

    private float lastTimeShot = 0;

    public PlayerAttributes playerAttributes;

    public List<Effects> bulletEffects;

    void Awake()
    {
        Instance = GetComponent<PlayerGun>();
        bulletEffects = new List<Effects>();
    }

    public bool Shoot()
    {
        return ShootDamage(playerAttributes.damage);
    }

    public bool ShootMagicianCoin()
    {
        return ShootDamage(playerAttributes.damage * Magician_Reward.damageMultiplier);
    }

    // Returns whether the shot was fired
    // Clones projectile and sets projectile stats
    bool ShootDamage(int damage)
    {   
        firingSpeed = 1/playerAttributes.attackPerSec;
        if(lastTimeShot + firingSpeed <= Time.time)
        {
            int currentAngle = (playerAttributes.bullets - 1) * 5;
            for (int i = 0; i < playerAttributes.bullets; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, Quaternion.Euler(0, 0, currentAngle + Random.Range(-playerAttributes.bulletSpread, playerAttributes.bulletSpread)) * firingPoint.rotation);
                projectile.GetComponent<Projectile>().setBulletBounce(playerAttributes.bulletBounces);
                projectile.GetComponent<Projectile>().setProjectileDamage(damage);
                foreach (Effects effect in bulletEffects)
                {
                    projectile.GetComponent<Projectile>().setBulletEffect(effect);
                }
                projectile.GetComponent<Projectile>().setBulletPiercing(playerAttributes.bulletPierces);
                currentAngle -= 10;
            }
            
            lastTimeShot = Time.time;
            return true;
        }
        return false;
    }

    public void SetBulletBounce(int n)
    {
        playerAttributes.bulletBounces = n;
    }

    public void SetBulletPierces(int n) { playerAttributes.bulletPierces = n; }

    public void AddEffect(Effects effect)
    {
        bulletEffects.Add(effect);
    }

    public void RemoveEffect(Effects effect)
    {
        bulletEffects.Remove(effect);
    }
}
