using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public static class StaticGameManager
{
    // PLAYER STATS SAVED BETWEEN PLAYTHROUGHS
    public static int futureSkillSlots = 3; // between 3-7

    // NARRATIVE ROOM GENERATION
    public static int deathCount = 0; // number of deaths so far
    public static RepeatDeathRooms.DeathCauses latestDeathCause = RepeatDeathRooms.DeathCauses.ScriptedDeath;
    // all counters include the current instance (1-indexed instead of 0-indexed)
    public static int roomCount = 0; // number of rooms encountered on this playthrough (including current room)
    public static int pathCount = 0; // number of NARRATIVE PATHS done (not playthroughs)
    static readonly Dictionary<int, int> visitCount = new(); // number of times each room was visited (inclusive)

    private static AsyncOperation asyncSceneLoad;

    public static void StartNewNarrativePath()
    {
        visitCount.Clear();
        ++pathCount;
        IncrementVisits();
    }

    public static bool VisitEquals(int room, int nvisit)
    {
        if (visitCount.ContainsKey(room))
        {
            return visitCount[room] == nvisit;
        }
        return false;
    }

    public static void IncrementVisits()
    {
        if (!visitCount.TryAdd(roomCount, 1))
        {
            ++visitCount[roomCount];
        }
    }

    public static void PreloadDeathScreen()
    {
        PreloadScene("SpiritPlane");
    }

    public static void PreloadPlayableScene()
    {
        PreloadScene("First Playable");
    }

    private static void PreloadScene(string sceneName)
    {
        asyncSceneLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncSceneLoad.allowSceneActivation = false;
    }

    public static void LoadDeathScreen()
    {
        asyncSceneLoad.allowSceneActivation = true;
    }

    public static void LoadPlayable()
    {
        asyncSceneLoad.allowSceneActivation = true;
    }
}