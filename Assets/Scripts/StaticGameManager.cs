using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class StaticGameManager
{
    // NARRATIVE ROOM GENERATION
    public static int deathCount = 0; // number of deaths so far
    // all counters include the current instance (1-indexed instead of 0-indexed)
    public static int roomCount = 0; // number of rooms encountered on this playthrough (including current room)
    public static int pathCount = 0; // number of NARRATIVE PATHS done (not playthroughs)
    public static Dictionary<(int, int), int> visitCount = new(); // number of times the node (path, room) was visited (inclusive)

    public static bool VisitEquals(int room, int path, int nvisit)
    {
        if (visitCount.ContainsKey((path, room)))
        {
            return visitCount[(path, room)] == nvisit;
        }
        return false;
    }

    public static void IncrementVisits()
    {
        if (!visitCount.TryAdd((pathCount, roomCount), 1))
        {
            ++visitCount[(pathCount, roomCount)];
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