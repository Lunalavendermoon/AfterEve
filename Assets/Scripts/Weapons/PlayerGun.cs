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
            PlayerAnimation animation = PlayerController.instance.playerAnimation;
            Vector3 shotOrigin;
            float baseAngle;
            Quaternion forwardOffset = Quaternion.Euler(0f, 90f, 0f);
            if (animation == null || !animation.TryGetArmShot(out shotOrigin, out baseAngle))
            {
                shotOrigin = firingPoint.position;
                if (!PlayerAnimation.TryGetMouseWorldPosition(shotOrigin.z, out Vector3 mouseWorldPos))
                    return false;

                Vector2 direction = mouseWorldPos - shotOrigin;
                baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                forwardOffset = firingPoint.rotation;
            }

            float spread = 10f;
            int bulletCount = PlayerController.instance.playerAttributes.bullets;

            float startAngle = baseAngle - spread * (bulletCount - 1) / 2f;
            for (int i = 0; i < PlayerController.instance.playerAttributes.bullets; i++)
            {
                float currentAngle = startAngle + spread * i;

                Quaternion projectileRotation = Quaternion.Euler(0, 0, currentAngle + Random.Range(-PlayerController.instance.playerAttributes.bulletSpread, PlayerController.instance.playerAttributes.bulletSpread));

                // Projectile reads its 2D travel direction from the XY components of transform.forward.
                projectileRotation *= forwardOffset;

                GameObject projectile = Instantiate(projectilePrefab, shotOrigin, projectileRotation);
                
                Projectile proj = projectile.GetComponent<Projectile>();
                proj.SetBulletBounce(PlayerController.instance.playerAttributes.bulletBounces);

                if (PlayerController.instance.IsInSpiritualVision())
                {
                    proj.SetSpiritualDamage(damage);
                }
                else
                {
                    proj.SetPhysicalDamage(damage);
                }

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
