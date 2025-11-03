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

    void Awake()
    {
        Instance = GetComponent<PlayerGun>();
    }

    public void Shoot()
    {
        firingSpeed = 1/playerAttributes.attackPerSec;
        if(lastTimeShot + firingSpeed <= Time.time)
        {
            int currentAngle = (playerAttributes.bullets - 1) * 5;
            for (int i = 0; i < playerAttributes.bullets; i++)
            {
                Instantiate(projectilePrefab, firingPoint.position, Quaternion.Euler(0, 0, currentAngle + Random.Range(-playerAttributes.bulletSpread, playerAttributes.bulletSpread)) * firingPoint.rotation);
                currentAngle -= 10;
            }
            
            lastTimeShot = Time.time;
        }
        
    }
}
