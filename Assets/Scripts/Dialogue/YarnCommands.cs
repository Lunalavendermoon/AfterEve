using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{
    private DialogueRunner runner;

    private void Awake()
    {
        runner = FindAnyObjectByType<DialogueRunner>();

        if (runner == null)
            Debug.LogError("No DialogueRunner found in the scene!");
    }

    [YarnCommand("set_portrait")]
    public void SetPortraitInYarnSpinner(string characterName, string spriteName = "Default")
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
        PlayerController.instance.EnablePlayerInput();
    }

    [YarnCommand("queue_next_dialogue")] // used to play dialogue mid combat
    public void WaitForNextDialogue(float seconds, string nextYarnNode)
    {
        Debug.Log($"Queuing next dialogue node '{nextYarnNode}' after {seconds} seconds.");
        StartCoroutine(WaitCoroutine(seconds, nextYarnNode));
    }

    private IEnumerator WaitCoroutine(float seconds, string nextYarnNode)
    {
        yield return new WaitForSeconds(seconds);
        PlayerController.instance.DisablePlayerInput();
        runner.StartDialogue(nextYarnNode);
    }
}
