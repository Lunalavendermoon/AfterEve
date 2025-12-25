using UnityEngine;

public class InteractableNPC : InteractableEntity
{
    public override void TriggerInteraction()
    {
        PlayerController.instance.playerInput.Disable();
        runner.StartDialogue(yarnSpinnerNode);
    }
}