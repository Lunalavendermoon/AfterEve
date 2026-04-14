using Pathfinding;
using UnityEngine;

public class Chariot_Pulse : MonoBehaviour
{
    const float chariotForce = 3f;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector2 disp = other.gameObject.transform.position - PlayerController.instance.transform.position;
            disp.Normalize();

            disp *= chariotForce * Time.fixedDeltaTime;

            AIBase ai = other.gameObject.GetComponent<AIBase>();
            ai.Move(disp);
        }
    }
}