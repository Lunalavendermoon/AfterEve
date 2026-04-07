using UnityEngine;
using TMPro;

public class InteractablePortal : InteractableEntity
{
    public GameManager gm;

    public override void TriggerInteraction()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.walkThroughPortal, this.transform.position);
        gm.LoadMap();
    }

    protected override string GetInteractionType()
    {
        return "Portal";
    }
}