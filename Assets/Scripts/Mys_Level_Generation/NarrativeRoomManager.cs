using System;
using UnityEngine;

public class NarrativeRoomManager : MonoBehaviour
{
    public static NarrativeRoomManager instance;

    public NarrativeRooms narrativeRooms;
    public GameObject portal;

    // TODO: we might want to put these data in a static class so it doesn't get reset as easily
    // all counters include the current instance (1-indexed instead of 0-indexed)
    public int roomCount = 0; // number of rooms encountered on this playthrough (including current room)
    int furthestRoom = 0; // furthest room encountered on current narrative path (resets when we start a new path)
    int pathCount = 0; // number of NARRATIVE PATHS done (not playthroughs)

    void Start()
    {
        if (instance == null) instance = this;
    }

    public void StartNewRoom()
    {
        ++roomCount;
    }

    public void StartNewCycle()
    {
        furthestRoom = Math.Max(furthestRoom, roomCount);
        roomCount = 0;
    }

    public void StartNewNarrativePath()
    {
        furthestRoom = 0;
        ++pathCount;
    }

    // If the current room and cycle count corresponds to a narrative room, spawn the room and return true.
    // Otherwise, do nothing and return false.
    public bool TrySpawnNarrativeRoom(Transform mapRoot)
    {
        // O(N) search... surely we won't have more than like 1000 narrative rooms right *copium*
        foreach (SingleNarrativeRoom room in narrativeRooms.rooms)
        {
            if (roomCount != room.roomCount || pathCount != room.pathCount)
            {
                continue;
            }

            // assumes that there are only 3 types of narrative room: single time, alternate, and repeat

            if (furthestRoom < roomCount)
            {
                // we haven't seen this room yet -- singleTime and repeat rooms are ok, but alternate is not
                if (room.nodeType == SingleNarrativeRoom.NodeType.Alternate)
                {
                    continue;
                }
            }
            else
            {
                // we've seen this room already -- alternate and repeat are ok, but singleTime is not
                if (room.nodeType == SingleNarrativeRoom.NodeType.SingleTime)
                {
                    continue;
                }
            }

            GameObject roomObj = Instantiate(
                room.roomPrefab,
                new Vector3(0f, 0f, 0f),
                Quaternion.identity,
                mapRoot
            );
            Instantiate(
                room.itemPrefab,
                new Vector3(0f, 0f, 0f),
                Quaternion.identity,
                roomObj.transform
            );
            // TODO portal position should be different in each narrative room
            portal.transform.position = roomObj.transform.position + new Vector3(2f, 2f, 0f);
            return true;
        }
        return false;
    }
}