using Pathfinding;
using UnityEngine;

public class Empress_Pulse : MonoBehaviour
{
    const float empressForce = 2f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector2 disp = other.gameObject.transform.position - PlayerController.instance.transform.position;
            disp.Normalize();

            disp *= empressForce;

            // Vector2 oldPos = new();
            // oldPos.x = other.gameObject.transform.position.x;
            // oldPos.y = other.gameObject.transform.position.y;

            // Vector2 newPos = oldPos + disp * empressForce;

            Debug.Log(
                $"Current position {other.gameObject.transform.position}, displacement {disp}");
            
            // TODO: Move() is a bit jittery LOL
            AIBase ai = other.gameObject.GetComponent<AIBase>();
            ai.Move(disp);
        }
    }
}