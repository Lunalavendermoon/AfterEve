using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using Yarn.Unity;

public class NarrativeRoomManager : MonoBehaviour
{
    public enum NarrativeRoomNeed
    {
        Nothing,
        MapOnly,
        All
    }

    public static NarrativeRoomManager instance;

    public AllNarrativePaths narrativePaths;
    public GameObject portal;
    public GameObject defaultRoom; // Spawn this room for CG nodes so player doesn't die standing on hot ground
    [HideInInspector] public bool disableChestGeneration;
    [HideInInspector] public bool hasCombat = false;
    [HideInInspector] public SingleNarrativeRoom currentRoom = null;
    [HideInInspector] public GameObject roomObject = null;
    [HideInInspector] public List<string> currentRoute = new();

    Transform mapRoot;

    bool needsEnemySpawn = false;

    // dialogue
    DialogueRunner runner;

    void Awake()
    {
        if (instance == null) instance = this;

        runner = FindAnyObjectByType<DialogueRunner>();

        if (runner == null)
            Debug.LogError("No DialogueRunner found in the scene!");
        
        GameManager.OnRoomChange += OnRoomChange;
        EnemySpawnerScript.OnAllEnemiesDefeated += OnAllEnemiesDefeated;

        runner.onDialogueComplete.AddListener(OnDialogueEnded);
    }

    void OnDisable()
    {
        GameManager.OnRoomChange -= OnRoomChange;
        EnemySpawnerScript.OnAllEnemiesDefeated -= OnAllEnemiesDefeated;
    }

    public void StartNewRoom()
    {
        ++StaticGameManager.roomCount;
        StaticGameManager.IncrementVisits();
    }

    public void StartNewCycle()
    {
        currentRoute.Clear();
        StaticGameManager.roomCount = 0;
        StaticGameManager.IncrementVisits();
    }

    // If the current room and cycle count corresponds to a narrative room, spawn the room and return true.
    // Otherwise, do nothing and return false.
    public NarrativeRoomNeed TrySpawnNarrativeRoom(Transform mapRoot)
    {
        if (StaticGameManager.pathCount == 0)
        {
            // Ensures pathCount is always at least 1
            // TODO delete this in final ver - this is a bandaid solution ONLY FOR EASE OF PLAYTESTING!!!
            StaticGameManager.StartNewNarrativePath();
        }

        currentRoom = null;
        roomObject = null;
        disableChestGeneration = false;

        if (StaticGameManager.pathCount > narrativePaths.paths.Count)
        {
            return NarrativeRoomNeed.All;
        }

        // O(N) search... surely we won't have more than like 20 narrative rooms per path right...
        foreach (SingleNarrativeRoom room in narrativePaths.paths[StaticGameManager.pathCount - 1].rooms)
        {
            if (StaticGameManager.roomCount != room.roomCount)
            {
                continue;
            }

            if (room.nodeType == SingleNarrativeRoom.NodeType.SingleTime &&
                !StaticGameManager.VisitEquals(room.roomCount, room.visitCount))
            {
                // player has visited the current room either too few or too many times to trigger dialogue
                continue;
            }
            else if (room.nodeType == SingleNarrativeRoom.NodeType.Branch &&
                     !currentRoute.Contains(room.routeName))
            {
                // Player is not on the correct route to trigger this scene
                continue;
            }

            currentRoom = room;

            switch (room.roomEnemyGenSetting)
            {
                // Randomized room and enemy spawns
                case SingleNarrativeRoom.RoomEnemyGen.Randomized:
                    portal.SetActive(false);
                    disableChestGeneration = room.disableChestGeneration;
                    needsEnemySpawn = true;
                    hasCombat = true;
                    return NarrativeRoomNeed.MapOnly;

                // Custom room and enemy spawn (used for tutorial & boss)
                case SingleNarrativeRoom.RoomEnemyGen.Custom:
                    if (room.roomPrefab == null)
                    {
                        Debug.LogWarning($"No custom room prefab found for room {room.roomCount}");
                        return NarrativeRoomNeed.All;
                    }
                    roomObject = Instantiate(
                        room.roomPrefab,
                        new Vector3(0f, 0f, 0f),
                        Quaternion.identity,
                        mapRoot
                    );
                    if (room.itemPrefab)
                    {
                        Instantiate(
                            room.itemPrefab,
                            new Vector3(0f, 0f, 0f),
                            Quaternion.identity,
                            roomObject.transform
                        );
                    }
                    portal.SetActive(false);
                    disableChestGeneration = room.disableChestGeneration;
                    needsEnemySpawn = room.enemyPrefabs.Count != 0;
                    hasCombat = needsEnemySpawn;
                    return NarrativeRoomNeed.Nothing;
                
                case SingleNarrativeRoom.RoomEnemyGen.P4Decisions:
                case SingleNarrativeRoom.RoomEnemyGen.P4LunaSearch:
                    this.mapRoot = mapRoot;
                    portal.SetActive(false);
                    disableChestGeneration = room.disableChestGeneration;
                    needsEnemySpawn = true;
                    hasCombat = true;
                    return NarrativeRoomNeed.MapOnly;

                // TODO: this should be deprecated eventually
                // Rooms that only play dialogue and then teleport player to next room
                case SingleNarrativeRoom.RoomEnemyGen.CutsceneOnly:
                    roomObject = Instantiate(
                        defaultRoom,
                        new Vector3(0f, 0f, 0f),
                        Quaternion.identity,
                        mapRoot
                    );
                    needsEnemySpawn = false;
                    hasCombat = false;
                    return NarrativeRoomNeed.Nothing;
            }
        }
        return NarrativeRoomNeed.All;
    }

    public void SpawnEnemies()
    {
        if (currentRoom)
        {
            needsEnemySpawn = false;
            if (currentRoom.roomEnemyGenSetting == SingleNarrativeRoom.RoomEnemyGen.Custom)
            {
                EnemySpawnerScript.instance.SpawnCustomEnemies(currentRoom.enemyPrefabs, roomObject);
            }
            else if ((currentRoom.roomEnemyGenSetting == SingleNarrativeRoom.RoomEnemyGen.P4Decisions &&
                    currentRoute.Contains("Decisions_Confront")) ||
                (currentRoom.roomEnemyGenSetting == SingleNarrativeRoom.RoomEnemyGen.P4LunaSearch &&
                    currentRoute.Contains("LunaSearch_Confront")))
            {
                GameManager.instance.ClearMap();
                roomObject = Instantiate(
                    currentRoom.roomPrefab,
                    new Vector3(0f, 0f, 0f),
                    Quaternion.identity,
                    mapRoot
                );
                EnemySpawnerScript.instance.SpawnCustomEnemies(currentRoom.enemyPrefabs, roomObject);
            }
            else
            {
                EnemySpawnerScript.instance.SpawnAllEnemies();
            }
        }
    }

    void OnRoomChange()
    {
        // When spawning in a new room, play on-spawn dialogue & BGM if it exists
        if (currentRoom && currentRoom.onSpawnDialogue.Length > 0)
        {
            PlayerController.instance.DisablePlayerInput();
            runner.StartDialogue(currentRoom.onSpawnDialogue);
            // TODO: just for testing, can remove this try/catch wrapper once all audio is implemented
            try
            {
                AudioManager.instance.InitializeMusic(currentRoom.roomMusic);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
    }

    void OnAllEnemiesDefeated()
    {
        if (currentRoom)
        {
            if (currentRoom.postCombatDialogue.Length > 0)
            {
                PlayerController.instance.DisablePlayerInput();
                runner.StartDialogue(currentRoom.postCombatDialogue);
            }
            else if ((currentRoom.roomEnemyGenSetting == SingleNarrativeRoom.RoomEnemyGen.P4Decisions &&
                        currentRoute.Contains("Decisions_Confront")) ||
                    (currentRoom.roomEnemyGenSetting == SingleNarrativeRoom.RoomEnemyGen.P4LunaSearch &&
                        currentRoute.Contains("LunaSearch_Confront")))
            {
                // Scripted death that only triggers on certain routes
                PlayerController.instance.Die(DamageInstance.DamageSource.ScriptedDeath);
            }
        }
    }

    public bool IsPointInsideCurrentRoom(Vector2 candidate, float radius = 0f)
    {
        if (roomObject == null) return false;
        SpriteRenderer sr = roomObject.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return false;
        Bounds b = sr.bounds;
        return candidate.x - radius >= b.min.x && candidate.x + radius <= b.max.x &&
               candidate.y - radius >= b.min.y && candidate.y + radius <= b.max.y;
    }

    // Skip dialogue/combat -- only used for testing
    public void SkipButtonPressed()
    {
        if (runner.IsDialogueRunning)
        {
            runner.Stop();
        }
        else
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject go in enemies)
            {
                go.GetComponent<EnemyBase>().Die();
            }

            GameObject[] boss = GameObject.FindGameObjectsWithTag("Boss");
            foreach (GameObject go in boss)
            {
                go.GetComponent<BossBehaviourBase>().Die();
            }
        }
    }

    // Handle game behavior after dialogue finishes playing.
    public void OnDialogueEnded()
    {
        PortraitManager.instance.ClearPortrait();
        PortraitManager.instance.ClearCG();
        
        PlayerController.instance.EnablePlayerInput();

        if (needsEnemySpawn)
        {
            // If there is combat after dialogue, spawn enemies
            SpawnEnemies();
        }
        else
        {
            if (currentRoom.isScriptedDeath)
            {
                // Used for rooms at the end of a path where the player dies no matter what
                PlayerController.instance.Die(DamageInstance.DamageSource.ScriptedDeath);
                return;
            }
            if (currentRoom.roomEnemyGenSetting == SingleNarrativeRoom.RoomEnemyGen.CutsceneOnly)
            {
                // Automatically move player to next room for cutscene-only nodes
                GameManager.instance.LoadMap();
                return;
            }
            // Spawn the portal if needed, or if this room doesn't spawn a chest after enemies defeated
            if (currentRoom.spawnPortalAfterLastDialogue || disableChestGeneration)
            {
                GameManager.instance.ClearCombatRoom(hasCombat);
            }
        }
    }

    public bool TryGetCurrentRoomWorldBounds(out Bounds worldBounds)
    {
        worldBounds = default;
        if (roomObject == null) return false;
        SpriteRenderer sr = roomObject.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return false;
        worldBounds = sr.bounds;
        return true;
    }
}