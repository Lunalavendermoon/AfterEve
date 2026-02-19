using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    // make into singleton
    public static EnemySpawnerScript instance;

    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject enemyPrefab;
        public Transform spawnPoint;

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

    // Runtime entries for the current generated map only
    public List<EnemyEntry> enemyList = new(); // entries of enemy + spawn point
    public List<EnemyBase> enemies = new(); // list Instantiated enemies in the scene

    private Transform currentMapRoot;

    private int pendingChestCoins;

    public static event System.Action OnAllEnemiesDefeated;

    void Awake()
    {
        if (instance == null) instance = this;
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

    public void ProcessRoom(GameObject roomPrefab, int i)
    {
        GameObject spawnedEnemy;
        // use List<GameObject> enemyPrefabs to assign enemies to SpawnPoints in level room/tiles
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("ERROR: no enemy prefabs assigned in enemyPrefabs");
            return;
        }

        spawnedEnemy = enemyPrefabs[i % enemyPrefabs.Count];
        
        foreach(Transform child in roomPrefab.transform) {
            if(child.gameObject.name == "SpawnPoint") // might have a better way of doing this?
            {   
                AddEnemyEntry(spawnedEnemy, child);
                break;
            }
        }
    }

    public void SpawnCustomEnemies(List<GameObject> enemies, GameObject roomPrefab)
    {
        // use List<GameObject> enemyPrefabs to assign enemies to SpawnPoints in level room/tiles
        if (enemies == null || enemies.Count == 0)
        {
            Debug.LogError("ERROR: no enemy prefabs assigned in enemyPrefabs");
            return;
        }

        foreach (GameObject spawnedEnemy in enemies)
        {
            foreach(Transform child in roomPrefab.transform) {
                if(child.gameObject.name == "SpawnPoint") // might have a better way of doing this?
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
        foreach (var entry in enemyList)
        {
            if (entry.enemyPrefab == null || entry.spawnPoint == null)
            {
                Debug.LogWarning("EnemyEntry has missing prefab or spawn point.");
                continue;
            }

            GameObject enemyObj = Instantiate(
                entry.enemyPrefab,
                entry.spawnPoint.position,
                Quaternion.Euler(new Vector3(0, 0, 0)
            ));

            EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
            enemy.chest = chest;
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
        numberOfEnemies--;
        enemies.Remove(enemy);

        if (numberOfEnemies <= 0)
        {
            SpawnAndRevealChestAt(enemy != null ? enemy.transform.position : Vector3.zero);
        }
    }

    public void AddPendingChestCoins(int amount)
    {
        if (amount <= 0) return;
        pendingChestCoins += amount;
    }

    private void SpawnAndRevealChestAt(Vector3 worldPos)
    {
        OnAllEnemiesDefeated?.Invoke();
        if (NarrativeRoomManager.instance.disableChestGeneration)
        {
            return;
        }

        if (chest != null)
        {
            chest.transform.position = worldPos;
            chest.gameObject.SetActive(true);
            return;
        }

        if (chestPrefab == null)
        {
            Debug.LogWarning("No chestPrefab assigned on EnemySpawnerScript. Cannot spawn chest.");
            return;
        }

        var parent = currentMapRoot != null ? currentMapRoot : null;
        chest = Instantiate(chestPrefab, worldPos, Quaternion.identity, parent);

        if (pendingChestCoins > 0)
        {
            chest.AddCoins(pendingChestCoins);
        }

        chest.gameObject.SetActive(true);
    }


}
