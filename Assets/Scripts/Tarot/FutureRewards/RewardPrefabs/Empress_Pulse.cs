using Pathfinding;
using UnityEngine;

public class Empress_Pulse : MonoBehaviour
{
    const float empressForce = 2f;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector2 disp = other.gameObject.transform.position - PlayerController.instance.transform.position;
            disp.Normalize();

            disp *= empressForce * Time.fixedDeltaTime;
            
            AIBase ai = other.gameObject.GetComponent<AIBase>();
            ai.Move(disp);
        }
    }
}