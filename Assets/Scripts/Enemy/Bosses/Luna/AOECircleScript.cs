using UnityEngine;

public class AOECircleScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //checked if collison object was in layer player
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Apply Effects
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController.instance.TakeDamage(10,DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Physical);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Remove Effects
        }
    }
}
