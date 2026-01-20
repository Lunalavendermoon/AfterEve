using UnityEngine;

[CreateAssetMenu]
// Represents a single narrative room that spawns when story conditions are unlocked.
public class SingleNarrativeRoom : ScriptableObject
{
    public enum NodeType
    {
        SingleTime,
        Alternate,
        Repeat
    }
    public NodeType nodeType;
    public int roomCount;
    public int pathCount;
    public GameObject roomPrefab;
    public GameObject itemPrefab;
    // public GameObject portalLocation;
}