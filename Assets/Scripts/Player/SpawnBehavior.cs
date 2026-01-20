using UnityEngine;

public class SpawnBehavior : MonoBehaviour
{
    public GameObject map;
    public GameManager gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        // Get the first child of the map and get the spawnposition which is a child of that first map.
        GameObject firstMap = map.transform.GetChild(0).gameObject;
        Vector3 spawnPosition = firstMap.transform.GetChild(0).gameObject.transform.position;
        // Move the player to the spawn position
        transform.position = spawnPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if colliding with a portal in portal layer, call loadmap function from mapmanager
        if (collision.gameObject.layer == LayerMask.NameToLayer("Portal"))
        {
            Debug.Log("Collided with portal, loading new map");
            gm.LoadMap();
        }
    }

}
