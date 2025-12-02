using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    // make into singleton
    public static EnemySpawnerScript instance;

    public Chest_base chest;
    public GameObject Enemy;
    public int numberOfEnemies = 0;
    public List<EnemyBase> enemies = new();

    void Start()
    {
        if (instance == null) instance = this;
        
        Enemy = Instantiate(Enemy, new Vector3(0, 0, 15), Quaternion.Euler(new Vector3(0, 0, 0)));
        EnemyBase enemy = Enemy.GetComponent<EnemyBase>();
        // move this to a manager class later
        enemy.chest = chest;
        enemy.spawner = this;
        numberOfEnemies++;
        enemies.Add(enemy);
    }

    public void checkRevealChest()
    {
      if (numberOfEnemies <= 0)
        {
            chest.gameObject.SetActive(true);
        }
    }

    public void EnemyDie(EnemyBase enemy)
    {
        numberOfEnemies--;
        enemies.Remove(enemy);
        checkRevealChest();
    }


}
