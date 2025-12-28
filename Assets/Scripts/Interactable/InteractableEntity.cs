using UnityEngine;
using Yarn.Unity;

public abstract class InteractableEntity : MonoBehaviour
{
    public string yarnSpinnerNode; // the yarn spinner node to trigger on interaction

    protected DialogueRunner runner;

    private void Awake()
    {
        runner = FindAnyObjectByType<DialogueRunner>();

        if (runner == null)
            Debug.LogError("No DialogueRunner found in the scene!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered");
        if (other.CompareTag("Player"))
        {
            if (PlayerController.instance.currentInteractable == null)
                PlayerController.instance.currentInteractable = this;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player exited");
        if (other.CompareTag("Player"))
        {
            if (PlayerController.instance.currentInteractable == this)
                PlayerController.instance.currentInteractable = null;
        }
    }

    public abstract void TriggerInteraction();
}
