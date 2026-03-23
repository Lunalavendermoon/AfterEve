using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DeathDialogueManager : MonoBehaviour
{
    public DeathRooms rooms;

    DialogueRunner runner;

    SingleTimeDeathRoom currentSpecialRoom = null;

    void Awake()
    {
        runner = FindAnyObjectByType<DialogueRunner>();

        if (runner == null)
            Debug.LogError("No DialogueRunner found in the scene!");
        
        runner.onDialogueComplete.AddListener(OnDialogueEnded);
    }

    void Start()
    {
        StaticGameManager.PreloadPlayableScene();

        string dialogueNode = null;
        currentSpecialRoom = null;
        foreach (SingleTimeDeathRoom room in rooms.singleTime)
        {
            // Pick the first valid dialogue node if multiple are available
            if (room.CanTrigger())
            {
                currentSpecialRoom = room;
                dialogueNode = room.dialogueNode;
                break;
            }
        }
        runner.StartDialogue(dialogueNode ?? SelectRepeatDialogue());
    }

    string SelectRepeatDialogue()
    {
        List<string> possibleNodes = rooms.repeats.GetPossibleNodes();
        if (possibleNodes.Count == 0)
        {
            Debug.LogWarning("No applicable death dialogue found, defaulting to fallback");
            return "DeathRepeat_Fallback_1";
        }
        // Randomly choose one dialogue from all candidates
        return possibleNodes[Random.Range(0, possibleNodes.Count)];
    }

    public void SkipButtonPressed()
    {
        runner.Stop();
    }

    void OnDialogueEnded()
    {
        if (currentSpecialRoom != null && currentSpecialRoom.CanStartNewPath())
        {
            StaticGameManager.StartNewNarrativePath();
        }
        StaticGameManager.LoadPlayable();
    }
}
