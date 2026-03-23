using UnityEngine;
using TMPro;

public class SpawnBehavior : MonoBehaviour
{
    public GameObject map;
    public GameManager gm;

    [Header("UI")]
    [SerializeField] private TMP_Text teleportPrompt;

    private bool isInPortal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetTeleportPromptVisible(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInPortal)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Teleport key pressed, loading new map");
            gm.LoadMap();
        }
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
            isInPortal = true;
            SetTeleportPromptVisible(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Portal"))
        {
            isInPortal = false;
            SetTeleportPromptVisible(false);
        }
    }

    private void SetTeleportPromptVisible(bool visible)
    {
        if (teleportPrompt == null)
        {
            return;
        }

        teleportPrompt.text = "Press F to teleport";
        teleportPrompt.gameObject.SetActive(visible);
    }

}
