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

    public Effects bulletEffect;

    void Awake()
    {
        Instance = GetComponent<PlayerGun>();
        bulletEffect = null;
    }

    public void Shoot()
    {
        firingSpeed = 1/playerAttributes.attackPerSec;
        if(lastTimeShot + firingSpeed <= Time.time)
        {
            int currentAngle = (playerAttributes.bullets - 1) * 5;
            for (int i = 0; i < playerAttributes.bullets; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, Quaternion.Euler(0, 0, currentAngle + Random.Range(-playerAttributes.bulletSpread, playerAttributes.bulletSpread)) * firingPoint.rotation);
                projectile.GetComponent<Projectile>().setBulletBounce(playerAttributes.bulletBounces);
                projectile.GetComponent<Projectile>().setBulletEffect(bulletEffect);
                projectile.GetComponent<Projectile>().setBulletPiercing(playerAttributes.bulletPierces);
                currentAngle -= 10;
            }
            
            lastTimeShot = Time.time;
        }
        
    }

    public void setBulletBounce(int n)
    {
        playerAttributes.bulletBounces = n;
    }

    public void setBulletPierces(int n) { playerAttributes.bulletPierces = n; }

    public void setEffect(Effects effect)
    {
        bulletEffect = effect;
    }

}
