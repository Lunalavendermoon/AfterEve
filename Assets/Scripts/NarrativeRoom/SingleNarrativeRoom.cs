using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleNarrativeRoom", menuName = "Scriptable Objects/SingleNarrativeRoom")]
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

    [Header("Enemies")]
    public List<GameObject> enemyPrefabs;
    public bool disableChestGeneration;

    [Header("Dialogue Nodes")]
    public string onSpawnDialogue;
    public string postCombatDialogue;
}