using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "SingleNarrativeRoom", menuName = "Scriptable Objects/Narrative/SingleNarrativeRoom")]
// Represents a single narrative room that spawns when story conditions are unlocked.
public class SingleNarrativeRoom : ScriptableObject
{
    public enum NodeType
    {
        SingleTime,
        Branch, // Only triggers if routeName appears in currentRoute for the current run
        Repeat
    }

    public enum RoomEnemyGen
    {
        Randomized,
        Custom,
        P4Decisions, // Spawns Eve boss on confrontation branch, regular enemies otherwise
        P4LunaSearch, // Spawns Luna boss on confrontation, regular otherwise
        CutsceneOnly // TODO: this should be deprecated eventually and is just for testing
        // Merge non-combat narrative rooms into their neighbors if possible.
    }

    public NodeType nodeType;
    public RoomEnemyGen roomEnemyGenSetting;
    public int roomCount;
    public int visitCount;
    public GameObject roomPrefab;
    public GameObject itemPrefab;
    public List<Sprite> cgList;
    public EventReference roomMusic;

    [Header("Enemies")]
    public List<GameObject> enemyPrefabs;
    public bool disableChestGeneration;

    [Header("Dialogue Nodes")]
    public string onSpawnDialogue;
    public string postCombatDialogue;
    public bool isScriptedDeath;
    public bool spawnPortalAfterLastDialogue;

    [Header("Optional - branching nodes only")]
    public string routeName;
}