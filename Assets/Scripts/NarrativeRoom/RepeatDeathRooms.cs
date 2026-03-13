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
