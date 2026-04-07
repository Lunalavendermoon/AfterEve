using UnityEngine;
using TMPro;

public class InteractablePortal : InteractableEntity
{
    public GameManager gm;

    public override void TriggerInteraction()
    {
        gm.LoadMap();
    }

    protected override string GetInteractionType()
    {
        return "Portal";
    }
}