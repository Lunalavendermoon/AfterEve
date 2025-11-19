using UnityEngine;

public abstract class InteractableEntity : MonoBehaviour
{
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
