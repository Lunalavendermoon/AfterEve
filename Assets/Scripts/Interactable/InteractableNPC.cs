using UnityEngine;

public class InteractableNPC : InteractableEntity
{
    public override void TriggerInteraction()
    {
        // TODO add actual dialogue/shop
        Debug.Log("NPC interaction triggered");
    }
}