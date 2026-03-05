using UnityEngine;

[CreateAssetMenu(fileName = "DeathRoom", menuName = "Scriptable Objects/Narrative/SingleTimeDeathRoom")]
public class SingleTimeDeathRoom : ScriptableObject
{
    public bool isFirstResurrection;
    public int roomCount;
    public int pathCount;
    public string dialogueNode;

    public bool CanTrigger(int room, int path)
    {
        if (isFirstResurrection)
        {
            return path == pathCount;
        }
        return path == pathCount && room == roomCount;
    }
}
