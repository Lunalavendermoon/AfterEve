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

    public Chest_base chest;
    public GameObject Enemy;
    public int numberOfEnemies = 0;
    public List<GameObject> enemyPrefabs; // pre-set prefabs, to make new EnemyEntrys with each level tile's SpawnPoints
    public List<EnemyEntry> enemyList = new(); // entries of enemy + spawn point
    public List<EnemyBase> enemies = new(); // list Instantiated enemies in the scene

    void Awake()
    {
        if (instance == null) instance = this;
    }
    void Start()
    {
        SpawnAllEnemies();
    }

    public void ProcessRoom(GameObject roomPrefab, int i)
    {
        GameObject spawnedEnemy;
        // if List<GameObject> enemyPrefabs not seralized, use prefabs from EnemyEntrys already in enemyList
        if(enemyPrefabs.Count == 0)
        {
            if(enemyList.Count == 0)
            {
                Debug.Log("ERROR: no enemy prefabs assigned in either enemyPrefabs or enemyList");
                return;
            }

            spawnedEnemy = enemyList[i%enemyList.Count].enemyPrefab;
        }
        else
        {
            // use List<GameObject> enemyPrefabs to assign enemies to SpawnPoints in level room/tiles
            spawnedEnemy = enemyPrefabs[i%enemyPrefabs.Count];
        }
        
        foreach(Transform child in roomPrefab.transform) {
            if(child.gameObject.name == "SpawnPoint") // might have a better way of doing this?
            {   
                AddEnemyEntry(spawnedEnemy, child);
                break;
            }
        }
    }

    public void AddEnemyEntry(GameObject enemy, Transform spawnPt)
    {
        enemyList.Add(new EnemyEntry(enemy, spawnPt));
    }

    public void CheckRevealChest()
    {
      if (numberOfEnemies <= 0)
        {
            chest.gameObject.SetActive(true);
        }
    }

    void SpawnAllEnemies()
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
    }

    public void EnemyDie(EnemyBase enemy)
    {
        numberOfEnemies--;
        enemies.Remove(enemy);
        CheckRevealChest();
    }


}
