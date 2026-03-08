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
        foreach (SingleTimeDeathRoom room in rooms.singleTime)
        {
            if (room.CanTrigger())
            {
                runner.StartDialogue(room.dialogueNode);
            }
        }
    }
}
