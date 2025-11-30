using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    public Chest_base chest;
    public GameObject Enemy;
    public int numberOfEnemies = 0;
    void Start()
    {
        Enemy = Instantiate(Enemy, new Vector3(0, 0, 15), Quaternion.Euler(new Vector3(0, 0, 0)));
        EnemyBase enemy = Enemy.GetComponent<EnemyBase>();
        // move this to a manager class later
        enemy.chest = chest;
        enemy.spawner = this;
        numberOfEnemies++;
    }

    public void checkRevealChest()
    {
      if (numberOfEnemies <= 0)
        {
            chest.gameObject.SetActive(true);
        }
    }


}
