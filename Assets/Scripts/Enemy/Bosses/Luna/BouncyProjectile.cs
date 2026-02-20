using UnityEngine;

public class BouncyProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private float bounceCount = 0;
    private float speed = 0f;
    private Vector3 velocity = Vector3.zero;
    private float Damage;

    private Transform BossParent;
    bool canTurn = false;
    public float rotationSpeed = 1f;

    private void Initialize()
    {
        
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Damage = 40f;
        velocity = (PlayerController.instance.transform.position - transform.position).normalized * speed;
    }
    public void Fire(float speed)
    {
               this.speed = speed;
        velocity = (PlayerController.instance.transform.position - transform.position).normalized * speed;
        rb.linearVelocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (bounceCount >= 2)
        {
            Destroy(gameObject);
        }

        if (Vector3.Distance(BossParent.position, transform.position) > 15f)
        {
            Destroy(gameObject);
        }

        
        if (velocity != Vector3.zero)
        {
            canTurn = true;
        }
    }

    private void FixedUpdate()
    {
        if (canTurn)
        {
            Vector2 toPlayer = (PlayerController.instance.transform.position - transform.position).normalized;
            float speed = velocity.magnitude;

            float steerFactor = rotationSpeed * Time.fixedDeltaTime;

            Vector2 newDir = Vector2.Lerp(velocity.normalized, toPlayer, steerFactor).normalized;
            velocity = newDir * speed;
            rb.linearVelocity = velocity;

            if (velocity.sqrMagnitude > 0.0001f)
                transform.up = velocity; // make sprite face movement
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if clashes into an obstacle layer object bounce
        if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Vector3 surfaceNormal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(transform.forward, surfaceNormal);
            velocity=reflectedDirection*velocity.magnitude;
            bounceCount++;
        }

        
    }

    public void SetBoss(Transform Boss)
    {
        BossParent= Boss;
    }



}
