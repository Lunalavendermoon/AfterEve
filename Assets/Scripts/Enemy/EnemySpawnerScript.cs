using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    private const int EarlyNodeCount = 4;
    private const float EarlyMoonJellyfishChance = 0.7f;
    private const float LaterMoonJellyfishChance = 0.5f;
    private const int EnemiesPerSpawnPoint = 2;

    // make into singleton
    public static EnemySpawnerScript instance;

    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject enemyPrefab;
        public Transform spawnPoint;
        public bool isProceduralSpawn;

        public EnemyEntry(GameObject enemy, Transform spawn)
        {
            enemyPrefab = enemy;
            spawnPoint = spawn;
        }
    }

    [Header("Chest")]
    public Chest_base chestPrefab;
    public Chest_base chest;
    public GameObject Enemy;
    public int numberOfEnemies = 0;
    [Header("Enemies")]
    public List<GameObject> enemyPrefabs; // prefab pool

    [Header("Spawn Position Jitter")]
    [SerializeField] private float spawnJitterRadius = 1.25f;      // small random radius around SpawnPoint
    [SerializeField] private int spawnJitterAttempts = 20;         // retry count
    [SerializeField] private float spawnBlockCheckRadius = 0.35f;  // probe size for collision check

    // Runtime entries for the current generated map only
    public List<EnemyEntry> enemyList = new(); // entries of enemy + spawn point
    public List<EnemyBase> enemies = new(); // list Instantiated enemies in the scene

    private Transform currentMapRoot;

    private int pendingChestCoins;
    private int pendingTarotCards;

    private int blockedSpawnMask;

    public static event System.Action OnAllEnemiesDefeated;

    void Awake()
    {
        if (instance == null) instance = this;

        // Uses exactly these two layers
        blockedSpawnMask = LayerMask.GetMask("VoidHole", "Obstacle");
    }
    void Start()
    {
        //SpawnAllEnemies();
    }

    public void ResetForNewMap()
    {
        enemyList.Clear();
        enemies.Clear();
        numberOfEnemies = 0;
        pendingChestCoins = 0;

        if (chest != null)
        {
            Destroy(chest.gameObject);
        }

        chest = null;
        currentMapRoot = null;
    }

    public void AssignChestFromMapRoot(Transform mapRoot)
    {
        // Keep for compatibility: this now just stores the root we'll parent runtime objects under.
        currentMapRoot = mapRoot;
    }

    public void ProcessRoom(GameObject roomPrefab, int nodeNumber)
    {
        // use List<GameObject> enemyPrefabs to assign enemies to SpawnPoints in level room/tiles
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("ERROR: no enemy prefabs assigned in enemyPrefabs");
            return;
        }

        GameObject moonJellyfish = null;
        List<GameObject> otherEnemies = new();
        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab == null || prefab.TryGetComponent<Mystic_MirrorScript>(out _))
                continue;

            if (prefab.TryGetComponent<MoonJellyFish>(out _))
                moonJellyfish = prefab;
            else if (!otherEnemies.Contains(prefab))
                otherEnemies.Add(prefab);
        }

        if (moonJellyfish == null && otherEnemies.Count == 0)
            return;

        float moonJellyfishChance = nodeNumber <= EarlyNodeCount
            ? EarlyMoonJellyfishChance
            : LaterMoonJellyfishChance;

        foreach (Transform child in roomPrefab.transform)
        {
            if (child.name != "SpawnPoint")
                continue;

            for (int i = 0; i < EnemiesPerSpawnPoint; i++)
            {
                bool spawnMoonJellyfish = moonJellyfish != null &&
                    (otherEnemies.Count == 0 || Random.value < moonJellyfishChance);
                GameObject prefab = spawnMoonJellyfish
                    ? moonJellyfish
                    : otherEnemies[Random.Range(0, otherEnemies.Count)];
                enemyList.Add(new EnemyEntry(prefab, child) { isProceduralSpawn = true });
            }
        }
    }

    public void SpawnCustomEnemies(List<GameObject> enemies, GameObject roomPrefab)
    {
        // Don't spawn any previously queued random enemies
        enemyList.Clear();

        // use List<GameObject> enemyPrefabs to assign enemies to SpawnPoints in level room/tiles
        if (enemies == null || enemies.Count == 0)
        {
            Debug.LogError("ERROR: no enemy prefabs assigned in enemyPrefabs");
            return;
        }

        foreach (GameObject spawnedEnemy in enemies)
        {
            foreach (Transform child in roomPrefab.transform)
            {
                if (child.gameObject.name == "SpawnPoint") // might have a better way of doing this?
                {
                    AddEnemyEntry(spawnedEnemy, child);
                    break;
                }
            }
        }

        ScanMap();
        SpawnAllEnemies();
    }

    public void AddEnemyEntry(GameObject enemy, Transform spawnPt)
    {
        enemyList.Add(new EnemyEntry(enemy, spawnPt));
    }

    public void ScanMap()
    {
        AstarPath.active.Scan();

    }
    public void SpawnAllEnemies()
    {
        enemies.Clear();
        numberOfEnemies = 0;
        Dictionary<Transform, Vector3> firstSpawnPositions = new();

        foreach (var entry in enemyList)
        {
            if (entry.enemyPrefab == null || entry.spawnPoint == null)
            {
                Debug.LogWarning("EnemyEntry has missing prefab or spawn point.");
                continue;
            }

            Vector3? firstSpawnPosition = null;
            if (entry.isProceduralSpawn && firstSpawnPositions.TryGetValue(entry.spawnPoint, out Vector3 occupiedPosition))
                firstSpawnPosition = occupiedPosition;

            if (!TryGetSpawnNearPoint(entry.spawnPoint, firstSpawnPosition, out Vector3 spawnPos))
            {
                Debug.LogWarning($"Could not find valid spawn near '{entry.spawnPoint.name}' for '{entry.enemyPrefab.name}'.");
                continue;
            }

            GameObject enemyObj = Instantiate(
                entry.enemyPrefab,
                spawnPos,
                Quaternion.identity
            );

            EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
            if (enemy is StandardEnemyBase standardEnemy)
            {
                standardEnemy.chest = chest;
                standardEnemy.ApproachFromOffscreen = entry.isProceduralSpawn;
            }
            if (entry.isProceduralSpawn && !firstSpawnPosition.HasValue)
                firstSpawnPositions.Add(entry.spawnPoint, spawnPos);
            enemy.spawner = this;
            enemies.Add(enemy);

            numberOfEnemies++;
        }

        //if (enemyList.Count > 0 && chest != null)
        //{
        //    chest.transform.position = enemyList[enemyList.Count - 1].spawnPoint.position;
        //}
    }

    public void EnemyDie(EnemyBase enemy)
    {
        if (enemy != null)
        {
            if (!enemies.Remove(enemy))
            {
                return;
            }
        }

        numberOfEnemies = Mathf.Max(0, numberOfEnemies - 1);

        if (numberOfEnemies <= 0)
        {
            SpawnAndRevealChestAt(enemy != null ? enemy.transform.position : Vector3.zero);
        }
    }

    public void BossDie()
    {
        // TODO: if a boss room ever spawns chest, we need to call SpawnAndRevealChest
        //  but so far no bosses spawn chest
        OnAllEnemiesDefeated?.Invoke();
    }

    public void AddPendingChestCoins(int amount)
    {
        if (amount <= 0) return;
        pendingChestCoins += amount;
    }

    public void AddPendingChestTarots(int count)
    {
        if (count <= 0) return;
        pendingTarotCards += count;
    }

    private void SpawnAndRevealChestAt(Vector3 worldPos)
    {
        OnAllEnemiesDefeated?.Invoke();
        if (NarrativeRoomManager.instance.disableChestGeneration)
        {
            return;
        }

        Vector3 playerPos = PlayerController.instance.transform.position;
        playerPos.z = 0f;

        if (chest != null)
        {
            // chest.transform.position = worldPos;
            chest.transform.position = playerPos;
            chest.gameObject.SetActive(true);
            return;
        }

        if (chestPrefab == null)
        {
            Debug.LogWarning("No chestPrefab assigned on EnemySpawnerScript. Cannot spawn chest.");
            return;
        }

        var parent = currentMapRoot != null ? currentMapRoot : null;
        // chest = Instantiate(chestPrefab, worldPos, Quaternion.identity, parent);
        chest = Instantiate(chestPrefab, playerPos, Quaternion.identity, parent);

        if (pendingChestCoins > 0)
        {
            chest.AddCoins(pendingChestCoins);
        }

        if (pendingTarotCards > 0)
        {
            chest.AddTarotCards(pendingTarotCards);
        }

        chest.gameObject.SetActive(true);
    }


    private bool TryGetSpawnNearPoint(Transform spawnPoint, Vector3? firstSpawnPosition, out Vector3 spawnPos)
    {
        spawnPos = spawnPoint.position;
        spawnPos.z = 0f;

        for (int i = 0; i < spawnJitterAttempts; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnJitterRadius;
            Vector3 candidate = spawnPoint.position + new Vector3(offset.x, offset.y, 0f);
            candidate.z = 0f;

            // Reject if candidate overlaps VoidHole or Obstacle
            if (!IsSpawnPositionClear(candidate, firstSpawnPosition))
                continue;

            spawnPos = candidate;
            return true;
        }

        // fallback: exact spawn point if valid
        return IsSpawnPositionClear(spawnPos, firstSpawnPosition);
    }

    private bool IsSpawnPositionClear(Vector3 position, Vector3? firstSpawnPosition)
    {
        return (!firstSpawnPosition.HasValue ||
                Vector2.Distance(position, firstSpawnPosition.Value) >= 2f * spawnBlockCheckRadius) &&
            Physics2D.OverlapCircle(position, spawnBlockCheckRadius, blockedSpawnMask) == null;
    }
}
