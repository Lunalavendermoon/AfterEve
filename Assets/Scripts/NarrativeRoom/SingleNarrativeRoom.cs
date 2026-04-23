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
        Repeat
    }

    public enum RoomEnemyGen
    {
        Randomized,
        Custom,
        CutsceneOnly
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
}