using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DeathDialogueManager : MonoBehaviour
{
    public DeathRooms rooms;

    DialogueRunner runner;

    void Awake()
    {
        runner = FindAnyObjectByType<DialogueRunner>();

        if (runner == null)
            Debug.LogError("No DialogueRunner found in the scene!");
    }

    void Start()
    {
        StaticGameManager.PreloadPlayableScene();

        string dialogueNode = null;
        foreach (SingleTimeDeathRoom room in rooms.singleTime)
        {
            if (room.CanTrigger())
            {
                dialogueNode = room.dialogueNode;
            }
        }
        runner.StartDialogue(dialogueNode ?? SelectRepeatDialogue());
    }

    string SelectRepeatDialogue()
    {
        List<string> possibleNodes = new() { };
        foreach (RepeatDeathRoom room in rooms.repeats)
        {
            if (room.CanTrigger())
            {
                possibleNodes.Add(room.dialogueNode);
            }
        }
        if (possibleNodes.Count == 0)
        {
            Debug.LogWarning("No applicable death dialogue found, defaulting to fallback");
            return "Death_Fallback_Fortesting";
        }
        // Randomly choose one dialogue from all candidates
        return possibleNodes[Random.Range(0, possibleNodes.Count)];
    }

    public void SkipButtonPressed()
    {
        runner.Stop();
        StaticGameManager.LoadPlayable();
    }
}
