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
        if (other.CompareTag("Player"))
        {
            if (PlayerController.instance.currentInteractable == null)
            {
                PlayerController.instance.currentInteractable = this;
                HandlePlayerEnter();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerController.instance.currentInteractable == this)
            {
                PlayerController.instance.currentInteractable = null;
                HandlePlayerExit();
            }
        }
    }

    public abstract void TriggerInteraction();

    protected virtual void HandlePlayerEnter() {}
    protected virtual void HandlePlayerExit() {}
}
