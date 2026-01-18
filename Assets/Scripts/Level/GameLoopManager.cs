//Core Orchestrator
using System.Collections.Generic;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    [Header("Run Configuration")]
    public GameRunConfig runConfig;

    [Header("Core Systems")]
    public RoomManager roomManager;
    public RoomGenerator roomGenerator;
    public ElementFiller elementFiller;

    [Header("Level Configs")]
    public List<LevelConfig> battleLevels;
    public LevelConfig shopLevel;
    public LevelConfig treasureLevel;
    public LevelConfig bossLevel;

    private int currentRoomIndex = 0;

    private void Start()
    {
        //debug for testing purposes
        Debug.Log("GameLoopManager START");
        GenerateCurrentRoom();
    }

    public void GenerateCurrentRoom()
    {
        Debug.Log("GenerateCurrentRoom called");

        if (currentRoomIndex >= runConfig.roomSequence.Count)
        {
            Debug.Log("Run complete");
            return;
        }

        GameRoomKind roomKind = runConfig.roomSequence[currentRoomIndex];
        Debug.Log("Room kind: " + roomKind);

        LevelConfig config = GetLevelConfig(roomKind);
        Debug.Log("LevelConfig chosen: " + config.name);

        ApplyConfig(config);
        roomManager.GenerateCompleteLevel();
    }

    public void AdvanceRoom()
    {
        currentRoomIndex++;
        GenerateCurrentRoom();
    }

    //TESTING PURPOSES ONLY, U SHOULD TURN THIS OFF FOR ACTUAL GAMEPLAY
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            AdvanceRoom();
            Debug.Log("Advancing to next room");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("FORCE GENERATE");
            roomManager.GenerateCompleteLevel();
        }
    }
    


    private LevelConfig GetLevelConfig(GameRoomKind kind)
    {
        switch (kind)
        {
            case GameRoomKind.Battle:
                return battleLevels[Random.Range(0, battleLevels.Count)];

            case GameRoomKind.Shop:
                return shopLevel;

            case GameRoomKind.Treasure:
                return treasureLevel;

            case GameRoomKind.Boss:
                return bossLevel;

            default:
                return battleLevels[0];
        }
    }

    private void ApplyConfig(LevelConfig config)
    {
        Debug.Log("Applying config to generators");
        config.ApplyToRoomGenerator(roomGenerator);
        config.ApplyToElementFiller(elementFiller);
    }
}
