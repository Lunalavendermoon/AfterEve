using UnityEngine;

public class NarrativeRoomManager : MonoBehaviour
{
    public NarrativeRooms narrativeRooms;
    public GameObject portal;

    // TODO: we might want to put these data in a static class so it doesn't get reset as easily
    int roomCount = 0;
    int cycleCount = 0;

    public void StartNewRoom()
    {
        ++roomCount;
    }

    public void StartNewCycle()
    {
        roomCount = 0;
        ++cycleCount;
    }

    // If the current room and cycle count corresponds to a narrative room, spawn the room and return true.
    // Otherwise, do nothing and return false.
    public bool TrySpawnNarrativeRoom(Transform mapRoot)
    {
        // O(N) search... surely we won't have more than like 1000 narrative rooms right *copium*
        foreach (SingleNarrativeRoom room in narrativeRooms.rooms)
        {
            // assumes that there are only 3 types of narrative room: single time, alternate, and repeat
            if (roomCount == room.roomCount &&
                (cycleCount == room.cycleCount || room.nodeType != SingleNarrativeRoom.NodeType.SingleTime))
            {
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
                portal.transform.position = roomObj.transform.position + room.portalLocation.transform.position;
                return true;
            }
        }
        return false;
    }
}