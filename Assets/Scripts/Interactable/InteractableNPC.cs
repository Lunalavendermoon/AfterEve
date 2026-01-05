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

        PlayerController.instance.DisablePlayerInput();
        runner.StartDialogue(targetNode);
        firstInteraction = false;
    }
}