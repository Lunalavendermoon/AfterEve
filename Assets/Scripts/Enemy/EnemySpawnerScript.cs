using System.Collections.Generic;
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
    }

    

    public Chest_base chest;
    public GameObject Enemy;
    public int numberOfEnemies = 0;
    public List<EnemyEntry> enemyList = new();

    public List<EnemyBase> enemies = new();

    void Start()
    {
        //if (instance == null) instance = this;

        SpawnAllEnemies();
    }

    public void checkRevealChest()
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
        checkRevealChest();
    }


}
