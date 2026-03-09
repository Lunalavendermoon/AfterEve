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
        List<string> possibleNodes = new();
        foreach (RepeatDeathRoom room in rooms.repeats)
        {
            if (room.CanTrigger())
            {
                return room.dialogueNode;
            }
        }
        Debug.LogWarning("No applicable death dialogue found!");
        // TODO change to random fallback
        return "Death_Fallback_Fortesting";
    }
}
