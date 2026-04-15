using System;
using UnityEngine;
using Yarn.Unity;

public class NarrativeRoomManager : MonoBehaviour
{
    public static NarrativeRoomManager instance;

    public AllNarrativePaths narrativePaths;
    public GameObject portal;
    [HideInInspector] public bool disableChestGeneration;
    [HideInInspector] public bool hasCombat = false;
    SingleNarrativeRoom currentRoom = null;
    GameObject roomObject = null;

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
        StaticGameManager.roomCount = 0;
        StaticGameManager.IncrementVisits();
    }

    // If the current room and cycle count corresponds to a narrative room, spawn the room and return true.
    // Otherwise, do nothing and return false.
    public bool TrySpawnNarrativeRoom(Transform mapRoot)
    {
        if (StaticGameManager.pathCount == 0)
        {
            // Ensures pathCount is always at least 1
            // TODO delete this in final ver - this is a bandaid solution ONLY FOR EASE OF PLAYTESTING!!!
            StaticGameManager.StartNewNarrativePath();
        }

        if (StaticGameManager.pathCount > narrativePaths.paths.Count)
        {
            return false;
        }
        // O(N) search... surely we won't have more than like 20 narrative rooms per path right...
        foreach (SingleNarrativeRoom room in narrativePaths.paths[StaticGameManager.pathCount - 1].rooms)
        {
            if (StaticGameManager.roomCount != room.roomCount)
            {
                continue;
            }

            // assumes that there are only 2 types of narrative room: single time and repeat

            if (room.nodeType == SingleNarrativeRoom.NodeType.SingleTime &&
                !StaticGameManager.VisitEquals(room.roomCount, room.visitCount))
            {
                // player has visited the current room either too few or too many times to trigger dialogue
                continue;
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
            currentRoom = room;
            disableChestGeneration = room.disableChestGeneration;
            needsEnemySpawn = room.enemyPrefabs.Count != 0;
            hasCombat = needsEnemySpawn;
            return true;
        }
        currentRoom = null;
        disableChestGeneration = false;
        return false;
    }

    public void SpawnEnemies()
    {
        if (currentRoom)
        {
            needsEnemySpawn = false;
            EnemySpawnerScript.instance.SpawnCustomEnemies(currentRoom.enemyPrefabs, roomObject);
        }
    }

    void OnRoomChange()
    {
        if (currentRoom && currentRoom.onSpawnDialogue.Length > 0)
        {
            PlayerController.instance.DisablePlayerInput();
            runner.StartDialogue(currentRoom.onSpawnDialogue);
        }
    }

    void OnAllEnemiesDefeated()
    {
        if (currentRoom && currentRoom.postCombatDialogue.Length > 0)
        {
            PlayerController.instance.DisablePlayerInput();
            runner.StartDialogue(currentRoom.postCombatDialogue);
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

    public void OnDialogueEnded()
    {
        PortraitManager.instance.ClearPortrait();
        PlayerController.FindScenePlayer().EnablePlayerInput();

        if (needsEnemySpawn)
        {
            SpawnEnemies();
        }
        else
        {
            if (currentRoom.isScriptedDeath)
            {
                PlayerController.instance.Die(DamageInstance.DamageSource.ScriptedDeath);
                return;
            }
            // Spawn the portal
            // If this room has no enemies to spawn after dialogue OR if this room doesn't spawn a chest after enemies defeated
            if (currentRoom.enemyPrefabs.Count == 0 || disableChestGeneration)
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