using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RepeatDeathRooms", menuName = "Scriptable Objects/Narrative/RepeatDeathRooms")]
public class RepeatDeathRooms : ScriptableObject
{
    public enum DeathCauses
    {
        Enemy,
        FalseHumanP1,
        FalseHumanP2,
        PossessedEve,
        Luna,
        Fallback,
        ScriptedDeath
    }
    public List<RepeatDeathDialoguePool> dialoguePools;

    public List<string> GetPossibleNodes()
    {
        foreach (RepeatDeathDialoguePool pool in dialoguePools)
        {
            if (CanTrigger(pool.cause))
            {
                return pool.dialogueNodes;
            }
        }
        return new();
    }

    bool CanTrigger(DeathCauses cause)
    {
        switch (cause)
        {
            case DeathCauses.FalseHumanP1:
                return StaticGameManager.roomCount == 10 && StaticGameManager.pathCount == 1;
            case DeathCauses.FalseHumanP2:
                return StaticGameManager.roomCount == 10 && StaticGameManager.pathCount == 2;
            case DeathCauses.PossessedEve:
                // TODO: add path 5 condition
                return (StaticGameManager.roomCount == 10 && StaticGameManager.pathCount == 3) ||
                        (StaticGameManager.roomCount == 1 && StaticGameManager.pathCount == 4 &&
                            NarrativeRoomManager.instance.currentRoute.Contains("Decisions_Confront"));
            case DeathCauses.Luna:
                // TODO: add path 5 condition
                return StaticGameManager.roomCount == 4 && StaticGameManager.pathCount == 4 &&
                        NarrativeRoomManager.instance.currentRoute.Contains("LunaSearch_Confront");
            case DeathCauses.Enemy:
                return cause == StaticGameManager.latestDeathCause;
            default:
                return true;
        }
    }
}

[Serializable]
public class RepeatDeathDialoguePool
{
    public RepeatDeathRooms.DeathCauses cause;
    public List<string> dialogueNodes = new();
}
