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
        return ShootDamage(PlayerController.instance.playerAttributes.AbilityMultiplyAdditiveDamage(1));
    }

    public bool ShootMagicianCoin()
    {
        return ShootDamage(PlayerController.instance.playerAttributes.AbilityMultiplyAdditiveDamage(Magician_Reward.damageMultiplier));
    }

    // Returns whether the shot was fired
    // Clones projectile and sets projectile stats
    bool ShootDamage(int damage)
    {
        firingSpeed = 1/PlayerController.instance.playerAttributes.attackPerSec;
        if(lastTimeShot + firingSpeed <= Time.time)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            Vector2 dir = (mouseWorldPos - firingPoint.position).normalized;
            float currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - (PlayerController.instance.playerAttributes.bullets - 1) * 5;
            for (int i = 0; i < PlayerController.instance.playerAttributes.bullets; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, Quaternion.Euler(0, 0, currentAngle + Random.Range(-PlayerController.instance.playerAttributes.bulletSpread, PlayerController.instance.playerAttributes.bulletSpread)) * firingPoint.rotation);
                Projectile proj = projectile.GetComponent<Projectile>();
                proj.SetBulletBounce(PlayerController.instance.playerAttributes.bulletBounces);

                // TODO: check if player is in spiritual vision. if so, change this to spiritual damage
                proj.SetPhysicalDamage(damage);

                // add bonus damage
                proj.AddPhysicalDamage((int)(PlayerController.instance.playerAttributes.damage * PlayerController.instance.playerAttributes.physicalAdditionalDmg));
                proj.AddSpiritualDamage((int)(PlayerController.instance.playerAttributes.damage * PlayerController.instance.playerAttributes.spiritualAdditionalDmg));
                foreach (Effects effect in bulletEffects)
                {
                    proj.SetBulletEffects(bulletEffects);
                }
                proj.SetBulletPiercing(PlayerController.instance.playerAttributes.bulletPierces);

                // past tarot effects
                if (PlayerController.instance.playerAttributes.empressPast)
                {
                    proj.AddPhysicalDamage(Empress_Past.GetDamageBonus());
                }

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
