using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using Yarn.Unity;

public abstract class InteractableEntity : MonoBehaviour
{
    public string yarnSpinnerNode; // the yarn spinner node to trigger on interaction

    protected DialogueRunner runner;

    protected LocalizedString interactPromptString;

    [SerializeField] private TMP_Text interactPrompt;

    private void Awake()
    {
        runner = FindAnyObjectByType<DialogueRunner>();

        if (runner == null)
            Debug.LogError("No DialogueRunner found in the scene!");

        interactPromptString = new LocalizedString
        {
            TableReference = "InteractPrompts",
            TableEntryReference = GetInteractionType(),
            // TODO: if settings allows player to change keybinds, need to update this every time the keybind changes
            Arguments = new[] { PlayerController.instance.playerInput.Player.Interact.GetBindingDisplayString() }
        };
    }

    protected abstract string GetInteractionType();

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (player.currentInteractable == null)
        {
            player.currentInteractable = this;
            SetInteractPromptVisible(true);
            HandlePlayerEnter();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (player.currentInteractable == this)
        {
            player.currentInteractable = null;
            SetInteractPromptVisible(false);
            HandlePlayerExit();
        }
    }

    public abstract void TriggerInteraction();

    protected virtual void HandlePlayerEnter() {}
    protected virtual void HandlePlayerExit() {}

    private void SetInteractPromptVisible(bool visible)
    {
        PlayerController player = PlayerController.instance;
        if (player == null)
        {
            return;
        }

        player.interactPrompt.text = interactPromptString.GetLocalizedString();
        player.interactPrompt.gameObject.SetActive(visible);
    }
}
