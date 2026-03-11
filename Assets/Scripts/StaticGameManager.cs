using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public static class StaticGameManager
{
    // TODO just for testing
    public static bool startNewNarrativePath = true;

    // NARRATIVE ROOM GENERATION
    public static int deathCount = 0; // number of deaths so far
    public static RepeatDeathRoom.DeathCauses latestDeathCause = RepeatDeathRoom.DeathCauses.Fallback;
    // all counters include the current instance (1-indexed instead of 0-indexed)
    public static int roomCount = 0; // number of rooms encountered on this playthrough (including current room)
    public static int pathCount = 0; // number of NARRATIVE PATHS done (not playthroughs)
    public static Dictionary<int, int> visitCount = new(); // number of times each room was visited (inclusive)

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

    public static void LoadDeathScreen()
    {
        SceneManager.LoadScene("SpiritPlane");
    }

    public static void LoadPlayable()
    {
        SceneManager.LoadScene("First Playable");
    }
}