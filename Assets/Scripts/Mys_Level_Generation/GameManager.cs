using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Tiles")]
    // Prefab *types* to spawn (each has a SpriteRenderer on XY plane facing +Z)
    public List<GameObject> tilePrefabs;
    public Transform mapRoot;

    [Header("Generation")]
    [Min(1)] public int tileCount = 4;   // typically 3–4 tiles

    // Logical map: grid coord instance already in the scene
    private Dictionary<Vector2Int, GameObject> gridToInstance =
        new Dictionary<Vector2Int, GameObject>();

    // For picking random existing tiles
    private List<Vector2Int> occupiedCells = new List<Vector2Int>();

    // Grid directions (x => world X, y => world Y)
    private readonly Vector2Int[] dirGrid =
    {
        new Vector2Int( 1, 0 ), // right (+X)
        new Vector2Int(-1, 0 ), // left  (-X)
        new Vector2Int( 0, 1 ), // up    (+Y)
        new Vector2Int( 0,-1 )  // down  (-Y)
    };

    // ============================================================
    // PUBLIC – call from your UI button
    // ============================================================
    public void LoadMap()
    {
        ClearMap();
        GenerateAndPlace();

        // log all positions from occupiedCells
        for (int i = 0; i < occupiedCells.Count; i++)
        {
            Vector2Int cell = occupiedCells[i];
            GameObject instance = gridToInstance[cell];
            Vector3 pos = instance.transform.position;
            Debug.Log("Tile " + i + " at logical cell " + cell + " positioned at " + pos);
        }
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

        // ---------- 1. Spawn FIRST tile at origin ----------
        Vector2Int startCell = Vector2Int.zero;
        GameObject startPrefab = GetRandomPrefab();
        GameObject startInstance = Instantiate(
            startPrefab,
            new Vector3(0f, 0f, 0f),      // XY plane, Z = 0
            Quaternion.identity,
            mapRoot
        );

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
