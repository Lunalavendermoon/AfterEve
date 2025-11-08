using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    EnemyBase enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<EnemyBase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Projectile bullet = collision.gameObject.GetComponent<Projectile>();
        if (bullet != null)
        {
            enemy.TakeDamage(10);
            Destroy(collision.gameObject); // optional
        }
    }
}
