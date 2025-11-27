using UnityEngine;

public abstract class InteractableEntity : MonoBehaviour
{
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Entered");
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exited");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Exited");
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && PlayerController.instance.playerInput.Player.Interact.triggered)
        {
            // might need to use raycast. currently interaction can trigger through walls
            // or make sure the trigger isn't going through the wall lol
            TriggerInteraction();
        }
    }

    public abstract void TriggerInteraction();
}
