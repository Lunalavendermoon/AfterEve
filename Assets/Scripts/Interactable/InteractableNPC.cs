using UnityEngine;

// represents an NPC with dialogue
public class InteractableNPC : InteractableEntity
{
    public string nextInteractionNode;

    protected bool firstInteraction = true;

    public override void TriggerInteraction()
    {
        if (!firstInteraction && string.IsNullOrWhiteSpace(nextInteractionNode))
        {
            return;
        }

        string targetNode = firstInteraction ? yarnSpinnerNode : nextInteractionNode;

        PlayerController.instance.playerInput.Disable();
        Debug.Log($"Start dialogue {targetNode}");
        runner.StartDialogue(targetNode);
        firstInteraction = false;
    }
}