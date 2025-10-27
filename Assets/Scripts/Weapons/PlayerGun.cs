using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField]
    Transform firingPoint;

    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    float firingSpeed;

    public static PlayerGun Instance;

    private float lastTimeShot = 0;

    public PlayerAttributes playerAttributes;

    void Awake()
    {
        Instance = GetComponent<PlayerGun>();
    }

    public void Shoot()
    {
        if(lastTimeShot + firingSpeed <= Time.time)
        {
            Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
            lastTimeShot = Time.time;
        }
        
    }
}
