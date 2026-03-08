using UnityEngine;

[CreateAssetMenu(fileName = "SingleTimeDeathRoom", menuName = "Scriptable Objects/Narrative/SingleTimeDeathRoom")]
public class SingleTimeDeathRoom : ScriptableObject
{
    public enum Trigger
    {
        SpecificNode,
        DeathCount
    }
    public Trigger trigger;
    public int roomCount;
    public int pathCount;
    public int deathCount;
    public string dialogueNode;

    public bool CanTrigger()
    {
        if (trigger == Trigger.DeathCount)
        {
            return StaticGameManager.deathCount == deathCount;
        }
        return StaticGameManager.pathCount == pathCount && StaticGameManager.roomCount == roomCount;
    }
}
