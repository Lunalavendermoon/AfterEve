using UnityEngine;
using TMPro;

public class InteractablePortal : InteractableEntity
{
    public GameManager gm;
    [SerializeField] private TMP_Text teleportPrompt;

    public override void TriggerInteraction()
    {
        gm.LoadMap();
    }

    protected override void HandlePlayerEnter()
    {
        SetTeleportPromptVisible(true);
    }

    protected override void HandlePlayerExit()
    {
        SetTeleportPromptVisible(false);
    }

    private void SetTeleportPromptVisible(bool visible)
    {
        if (teleportPrompt == null)
        {
            return;
        }

        // TODO: use localized string asset
        teleportPrompt.text = "Press F to teleport";
        teleportPrompt.gameObject.SetActive(visible);
    }
}