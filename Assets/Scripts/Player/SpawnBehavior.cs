using UnityEngine;
using TMPro;

public class SpawnBehavior : MonoBehaviour
{
    public GameObject map;

    public void Respawn()
    {
        // Get the first child of the map and get the spawnposition which is a child of that first map.
        GameObject firstMap = map.transform.GetChild(0).gameObject;
        Vector3 spawnPosition = firstMap.transform.GetChild(0).gameObject.transform.position;
        // Move the player to the spawn position
        transform.position = spawnPosition;
    }
}
