using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    AnimationCurve height;
    Rigidbody2D rb;
    public float speed = 5f;
    public float explosionRadius = 2f;
    public int damage = 10;
    private float initialDistance;
    private bool isThrown = false;
    private GameObject grenadeMarker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        height = new AnimationCurve();
        height.AddKey(0f, 0f);
        height.AddKey(0.5f, 1f);
        height.AddKey(1f, 0f);
    }
    public void Throw(Vector2 target, GameObject grenadeMarker)
    {
        rb = GetComponent<Rigidbody2D>();
        initialDistance = Vector2.Distance(transform.position, target);
        this.grenadeMarker = grenadeMarker;
        isThrown = true;
    }

    private void FixedUpdate()
    {
        if (isThrown)
        {
            float distanceCovered = Vector2.Distance(transform.position, rb.position);
            if(initialDistance - distanceCovered < 0.5f)
            {
                Explode();
                return;
            }
            float heightOffset = height.Evaluate(distanceCovered / initialDistance);
            Vector2 direction = (rb.position - (Vector2)transform.position).normalized;
            Vector2 horizontalMovement = direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + horizontalMovement + Vector2.up * heightOffset);
        }
    }

    private void Explode()
    {
        // Instantiate explosion effect here
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerController.instance.TakeDamage(damage, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Physical);
            }
        }
        Destroy(grenadeMarker);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

            Explode();

    }
}



