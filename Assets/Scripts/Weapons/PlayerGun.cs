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


    public List<Effects> bulletEffects;

    void Awake()
    {
        Instance = GetComponent<PlayerGun>();
        bulletEffects = new List<Effects>();
    }

    public bool Shoot()
    {
        return ShootDamage(PlayerController.instance.playerAttributes.damage);
    }

    public bool ShootMagicianCoin()
    {
        return ShootDamage(PlayerController.instance.playerAttributes.damage * Magician_Reward.damageMultiplier);
    }

    // Returns whether the shot was fired
    // Clones projectile and sets projectile stats
    bool ShootDamage(int damage)
    {   
        firingSpeed = 1/PlayerController.instance.playerAttributes.attackPerSec;
        if(lastTimeShot + firingSpeed <= Time.time)
        {
            int currentAngle = (PlayerController.instance.playerAttributes.bullets - 1) * 5;
            for (int i = 0; i < PlayerController.instance.playerAttributes.bullets; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, Quaternion.Euler(0, 0, currentAngle + Random.Range(-PlayerController.instance.playerAttributes.bulletSpread, PlayerController.instance.playerAttributes.bulletSpread)) * firingPoint.rotation);
                projectile.GetComponent<Projectile>().SetBulletBounce(PlayerController.instance.playerAttributes.bulletBounces);
                projectile.GetComponent<Projectile>().SetProjectileDamage(damage);
                foreach (Effects effect in bulletEffects)
                {
                    projectile.GetComponent<Projectile>().SetBulletEffects(bulletEffects);
                }
                projectile.GetComponent<Projectile>().SetBulletPiercing(PlayerController.instance.playerAttributes.bulletPierces);
                currentAngle -= 10;
            }
            
            lastTimeShot = Time.time;
            return true;
        }
        return false;
    }

    public void SetBulletBounce(int n)
    {
        PlayerController.instance.playerAttributes.bulletBounces = n;
    }

    public void SetBulletPierces(int n) { PlayerController.instance.playerAttributes.bulletPierces = n; }

    public void AddEffect(Effects effect)
    {
        bulletEffects.Add(effect);
    }

    public void RemoveEffect(Effects effect)
    {
        bulletEffects.Remove(effect);
    }
}
