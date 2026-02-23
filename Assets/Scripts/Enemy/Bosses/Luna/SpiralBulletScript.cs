using UnityEngine;

public class SpiralBulletScript : MonoBehaviour
{
    Vector3 Direction;
    Vector3 origin;
    float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Rigidbody2D rb;
    private void Start()
    {
        rb= GetComponent<Rigidbody2D>();
    }

    public void InitializeProjectile(Vector2 DirectionRequired, Vector3 originEnemy, float speedOfBullet)
    {
        Direction = DirectionRequired;
        origin = originEnemy;
        speed = speedOfBullet;
        // rotate to make up = direction
        transform.up=Direction;

    }

    // Update is called once per frame
    void Update()
    {

        if (Vector2.Distance(origin, transform.position) > 20f) { 
            Destroy(gameObject);
            return;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null && rb.linearVelocity.magnitude<=0f)
            rb.linearVelocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController.instance.TakeDamage(10, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Physical);
            Destroy(gameObject);
            return;
        }
    }
}
