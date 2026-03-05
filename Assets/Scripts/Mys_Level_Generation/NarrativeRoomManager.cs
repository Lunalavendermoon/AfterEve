using System;
using UnityEngine;
using Yarn.Unity;

public class NarrativeRoomManager : MonoBehaviour
{
    public static NarrativeRoomManager instance;

    public NarrativeRooms narrativeRooms;
    public GameObject portal;
    [HideInInspector] public bool disableChestGeneration;
    SingleNarrativeRoom currentRoom = null;
    GameObject roomObject = null;

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

    public void StartNewNarrativePath()
    {
        ++StaticGameManager.pathCount;
        StaticGameManager.IncrementVisits();
    }

    // If the current room and cycle count corresponds to a narrative room, spawn the room and return true.
    // Otherwise, do nothing and return false.
    public bool TrySpawnNarrativeRoom(Transform mapRoot)
    {
        // O(N) search... surely we won't have more than like 1000 narrative rooms right *copium*
        foreach (SingleNarrativeRoom room in narrativeRooms.rooms)
        {
            if (StaticGameManager.roomCount != room.roomCount || StaticGameManager.pathCount != room.pathCount)
            {
                continue;
            }

            // assumes that there are only 2 types of narrative room: single time and repeat

            if (room.nodeType == SingleNarrativeRoom.NodeType.SingleTime &&
                !StaticGameManager.VisitEquals(room.roomCount, room.pathCount, room.visitCount))
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

            // TODO portal position should be different in each narrative room
            portal.transform.position = roomObject.transform.position + new Vector3(2f, 2f, 0f);
            portal.SetActive(false);
            currentRoom = room;
            disableChestGeneration = room.disableChestGeneration;
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
}