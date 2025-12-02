using UnityEngine;

public class Empress_Pulse : MonoBehaviour
{
    const float empressForce = 3f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 disp = other.gameObject.transform.position - PlayerController.instance.transform.position;
            disp.Normalize();

            Vector2 oldPos = new();
            oldPos.x = other.gameObject.transform.position.x;
            oldPos.y = other.gameObject.transform.position.y;

            Vector2 newPos = oldPos + disp * empressForce;

            Debug.Log(
                $"Current position {other.gameObject.transform.position} + displacement {disp * empressForce} = target position {newPos}");
            // TODO - will fix this
            rb.MovePosition(newPos);
        }
    }
}