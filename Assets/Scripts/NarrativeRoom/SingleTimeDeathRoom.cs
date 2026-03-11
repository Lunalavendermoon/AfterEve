using UnityEngine;

[CreateAssetMenu(fileName = "SingleTimeDeathRoom", menuName = "Scriptable Objects/Narrative/SingleTimeDeathRoom")]
public class SingleTimeDeathRoom : ScriptableObject
{
    public enum Trigger
    {
        SpecificNode,
        DeathCount,
        FirstResurrection
    }
    public Trigger trigger;
    public int roomCount;
    public int pathCount;
    public int deathCount;
    public string dialogueNode;
    public bool startsNewPath;

    public bool CanTrigger()
    {
        if (trigger == Trigger.DeathCount || trigger == Trigger.FirstResurrection)
        {
            return StaticGameManager.deathCount == deathCount;
        }
        return RoomCorrectAndBossDefeated();
    }

    public bool CanStartNewPath()
    {
        if (trigger == Trigger.FirstResurrection)
        {
            return RoomCorrectAndBossDefeated();
        }
        return startsNewPath;
    }

    // Special dialogue triggered by a specific node (as opposed to death count) is always caused by scripted death
    bool RoomCorrectAndBossDefeated()
    {
        return StaticGameManager.latestDeathCause == RepeatDeathRoom.DeathCauses.ScriptedDeath &&
            StaticGameManager.pathCount == pathCount && StaticGameManager.roomCount == roomCount;
    }
}
