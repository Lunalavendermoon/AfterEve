using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Tiles")]
    // Prefab *types* to spawn (each has a SpriteRenderer on XY plane facing +Z)
    public List<GameObject> tilePrefabs;
    public Transform mapRoot;

    [Header("Generation")]
    [Min(1)] public int tileCount = 4;   // typically 3�4 tiles

    // Logical map: grid coord instance already in the scene
    private Dictionary<Vector2Int, GameObject> gridToInstance =
        new Dictionary<Vector2Int, GameObject>();

    // For picking random existing tiles
    private List<Vector2Int> occupiedCells = new List<Vector2Int>();

    public SpawnBehavior spawnBehavior;
    public GameObject portal;

    [Header("Portal Placement")]
    [Min(0f)] public float portalSpawnRadius = 2f;
    [Min(1)] public int portalSpawnAttempts = 30;
    [Min(0f)] public float portalPlayerClearance = 0.75f;
    [Min(0f)] public float portalObstacleClearance = 0.05f;
    public LayerMask portalBlockingLayers;
    // Grid directions (x => world X, y => world Y)
    private readonly Vector2Int[] dirGrid =
    {
        new Vector2Int( 1, 0 ), // right (+X)
        new Vector2Int(-1, 0 ), // left  (-X)
        new Vector2Int( 0, 1 ), // up    (+Y)
        new Vector2Int( 0,-1 )  // down  (-Y)
    };

    [Header("Narrative Room Helper")]
    public NarrativeRoomManager narrativeRoomManager;

    // For testing only - use to toggle on/off narrative room generation
    public bool generateNarrativeRooms;

    // Events
    public static event System.Action OnRoomChange;
    public static event System.Action OnCombatRoomClear;

    private Bounds? mapBounds;

    void Awake()
    {
        if (instance == null) instance = this;

        if (generateNarrativeRooms)
        {
            narrativeRoomManager.StartNewNarrativePath(); // TODO: call this whenever player starts a new narrative path
        }
    }

    void Start()
    {
        // call in start so we have time to setup listeners
        StartNewPlaythrough();
    }

    public void StartNewPlaythrough()
    {
        if (generateNarrativeRooms)
        {
            narrativeRoomManager.StartNewCycle(); // TODO: call this whenever player starts a new run
        }
        LoadMap();
    }

    // ============================================================
    // PUBLIC � call from your UI button
    // ============================================================
    public void LoadMap()
    {
        // Reset spawner state before destroying the old map so it doesn't keep references
        // to spawn points/chests that are about to be destroyed.
        if (EnemySpawnerScript.instance != null)
        {
            EnemySpawnerScript.instance.ResetForNewMap();
        }
        ClearMap();
        GenerateAndPlace();
        OnRoomChange?.Invoke();
    }

    // ============================================================
    // Main generation loop: uses logical grid + existing instances
    // ============================================================
    private void GenerateAndPlace()
    {
        if (tilePrefabs == null || tilePrefabs.Count == 0)
        {
            Debug.LogError("No tilePrefabs assigned on GameManager.");
            return;
        }

        gridToInstance.Clear();
        occupiedCells.Clear();
        mapBounds = null;


        if (generateNarrativeRooms)
        {
            narrativeRoomManager.StartNewRoom();
            if (narrativeRoomManager.TrySpawnNarrativeRoom(mapRoot))
            {
                spawnBehavior.Respawn();
                return;
            }
        }

        // ---------- 1. Spawn FIRST tile at origin ----------
        Vector2Int startCell = Vector2Int.zero;
        GameObject startPrefab = GetRandomPrefab();
        GameObject startInstance = Instantiate(
            startPrefab,
            new Vector3(0f, 0f, 0f),      // XY plane, Z = 0
            Quaternion.identity,
            mapRoot
        );
        EnemySpawnerScript.instance.ProcessRoom(startInstance, 0);

        gridToInstance[startCell] = startInstance;
        occupiedCells.Add(startCell);

        // ---------- 2. Iteratively add neighbours ----------
        for (int i = 1; i < tileCount; i++)
        {
            // Collect cells that still have at least one free neighbour
            List<Vector2Int> expandableCells = new List<Vector2Int>();
            foreach (Vector2Int cell in occupiedCells)
            {
                if (HasFreeNeighbor(cell))
                    expandableCells.Add(cell);
            }

            if (expandableCells.Count == 0)
            {
                Debug.LogWarning("No expandable cells left; stopping early at " + i + " tiles.");
                break;
            }

            // Choose random base logical cell & its instance
            Vector2Int baseCell = expandableCells[Random.Range(0, expandableCells.Count)];
            GameObject baseInstance = gridToInstance[baseCell];

            // Find free directions from this base cell
            List<int> freeDirIndices = new List<int>();
            for (int d = 0; d < dirGrid.Length; d++)
            {
                Vector2Int neighborCell = baseCell + dirGrid[d];
                if (!gridToInstance.ContainsKey(neighborCell))
                    freeDirIndices.Add(d);
            }

            if (freeDirIndices.Count == 0)
            {
                // Shouldn't happen due to HasFreeNeighbor, but just in case
                continue;
            }

            // Pick random free direction
            int dirIndex = freeDirIndices[Random.Range(0, freeDirIndices.Count)];
            Vector2Int dir = dirGrid[dirIndex];
            Vector2Int newCell = baseCell + dir;

            // ---------- 3. Spawn new tile based on EXISTING instance ----------
            GameObject prefabToSpawn = GetRandomPrefab();
            GameObject newInstance = Instantiate(
                prefabToSpawn,
                baseInstance.transform.position,  // temp, we'll move it
                Quaternion.identity,
                mapRoot
            );
            EnemySpawnerScript.instance.ProcessRoom(newInstance, i);

            // Get render sizes (world units)
            Vector3 baseSize = GetSpriteSize(baseInstance);
            Vector3 newSize = GetSpriteSize(newInstance);

            Vector3 basePos = baseInstance.transform.position;
            Vector3 offset = Vector3.zero;

            // NOTE: size.x = width (X axis), size.y = height (Y axis), size.z  thickness

            if (dir.x == 1)        // right of base ( +X )
            {
                float dist = (baseSize.x * 0.5f) + (newSize.x * 0.5f);
                offset = new Vector3(dist, 0f, 0f);
            }
            else if (dir.x == -1)  // left of base ( -X )
            {
                float dist = (baseSize.x * 0.5f) + (newSize.x * 0.5f);
                offset = new Vector3(-dist, 0f, 0f);
            }
            else if (dir.y == 1)   // up of base ( +Y )
            {
                float dist = (baseSize.y * 0.5f) + (newSize.y * 0.5f);
                offset = new Vector3(0f, dist, 0f);
            }
            else if (dir.y == -1)  // down of base ( -Y )
            {
                float dist = (baseSize.y * 0.5f) + (newSize.y * 0.5f);
                offset = new Vector3(0f, -dist, 0f);
            }

            newInstance.transform.position = basePos + offset;

            // Force same Z for all tiles if you want, e.g. 0:
            Vector3 fixedPos = newInstance.transform.position;
            fixedPos.z = 0f;
            newInstance.transform.position = fixedPos;

            // ---------- 4. Register new instance in logical grid ----------
            gridToInstance[newCell] = newInstance;
            occupiedCells.Add(newCell);
        }

        // Provide mapRoot so runtime-spawned objects (e.g. chest) can be parented under it.
        if (EnemySpawnerScript.instance != null)
        {
            EnemySpawnerScript.instance.AssignChestFromMapRoot(mapRoot);
        }

        EnemySpawnerScript.instance.ScanMap();
        EnemySpawnerScript.instance.SpawnAllEnemies();
        portal.SetActive(false);

        spawnBehavior.Respawn();
    }

    public void ClearCombatRoom()
    {
        // Used by non-chest flows (e.g., narrative/Yarn rooms) to finish the room.
        // Spawn the portal on the player for immediate transition.
        if (portal == null)
        {
            Debug.LogWarning("GameManager portal reference is null.");
            return;
        }

        if (PlayerController.instance == null)
        {
            Debug.LogWarning("PlayerController.instance is null; cannot spawn portal on player.");
            portal.SetActive(true);
            return;
        }

        Vector3 playerPos = PlayerController.instance.transform.position;
        playerPos.z = 0f;
        portal.transform.position = playerPos;
        portal.SetActive(true);
    }

    public bool SpawnPortalNearChest(Vector3 chestWorldPos)
    {
        if (portal == null)
        {
            Debug.LogWarning("GameManager portal reference is null.");
            return false;
        }

        if (PlayerController.instance == null)
        {
            Debug.LogWarning("PlayerController.instance is null; cannot spawn portal on player.");
            return false;
        }

        Vector3 playerPos = PlayerController.instance.transform.position;
        playerPos.z = 0f;
        portal.transform.position = playerPos;
        portal.SetActive(true);
        return true;
    }

    private void EnsureMapBounds()
    {
        if (mapBounds != null) return;
        if (occupiedCells.Count == 0)
        {
            mapBounds = null;
            return;
        }

        bool hasAny = false;
        Bounds b = default;

        foreach (var cell in occupiedCells)
        {
            if (!gridToInstance.TryGetValue(cell, out var tile) || tile == null) continue;
            SpriteRenderer sr = tile.GetComponentInChildren<SpriteRenderer>();
            if (sr == null) continue;

            if (!hasAny)
            {
                b = sr.bounds;
                hasAny = true;
            }
            else
            {
                b.Encapsulate(sr.bounds);
            }
        }

        mapBounds = hasAny ? b : null;
    }

    private bool IsInsideMap(Vector2 candidate, float radius)
    {
        if (mapBounds == null) return false;
        Bounds b = mapBounds.Value;

        return candidate.x - radius >= b.min.x && candidate.x + radius <= b.max.x &&
               candidate.y - radius >= b.min.y && candidate.y + radius <= b.max.y;
    }

    private bool IsOverlappingBlocking(Vector2 candidate, float radius)
    {
        Vector2 size = new Vector2(radius * 2f, radius * 2f);
        Collider2D hit = Physics2D.OverlapBox(candidate, size, 0f, portalBlockingLayers);
        return hit != null;
    }

    private float GetApproxRadiusFromBounds(GameObject obj)
    {
        SpriteRenderer sr = obj != null ? obj.GetComponentInChildren<SpriteRenderer>() : null;
        if (sr != null)
        {
            Vector3 s = sr.bounds.size;
            return Mathf.Max(s.x, s.y) * 0.5f;
        }

        Collider2D col = obj != null ? obj.GetComponentInChildren<Collider2D>() : null;
        if (col != null)
        {
            Vector3 s = col.bounds.size;
            return Mathf.Max(s.x, s.y) * 0.5f;
        }

        return 0.5f;
    }

    // ============================================================
    // Helper: does this logical cell have ANY free neighbour?
    // ============================================================
    private bool HasFreeNeighbor(Vector2Int cell)
    {
        foreach (Vector2Int dir in dirGrid)
        {
            Vector2Int neighbor = cell + dir;
            if (!gridToInstance.ContainsKey(neighbor))
                return true;
        }
        return false;
    }

    // ============================================================
    // Helper: pick random prefab
    // ============================================================
    private GameObject GetRandomPrefab()
    {
        return tilePrefabs[Random.Range(0, tilePrefabs.Count)];
    }

    // ============================================================
    // Helper: SpriteRenderer bounds -> world size
    // ============================================================
    private Vector3 GetSpriteSize(GameObject instance)
    {
        SpriteRenderer sr = instance.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return new Vector3(1f, 1f, 1f);

        return sr.bounds.size; // XY size in world units
    }

    // ============================================================
    // Clear previously spawned tiles
    // ============================================================
    private void ClearMap()
    {
        gridToInstance.Clear();
        occupiedCells.Clear();

        if (mapRoot == null) return;

        for (int i = mapRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(mapRoot.GetChild(i).gameObject);
        }
    }
}
