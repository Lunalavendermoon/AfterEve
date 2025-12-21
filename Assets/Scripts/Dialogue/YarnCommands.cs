using UnityEngine;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{
    [YarnCommand("set_portrait")]
    public void SetPortraitInYarnSpinner(string characterName, string spriteName)
    {
        PortraitManager.instance.SetPortrait(characterName, spriteName);
    }

    [YarnCommand("clear_portrait")]
    public void ClearPortraitInYarnSpinner()
    {
        PortraitManager.instance.ClearPortrait();
    }


    [YarnCommand("start_controls")] // used to restart player controls after dialogue
    public void StartControls()
    {
        PlayerController.instance.playerInput.Enable();
    }
}
