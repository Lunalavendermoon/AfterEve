using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject Enemy;
    void Start()
    {
        Enemy = Instantiate(Enemy, new Vector3(0, 0, 15), Quaternion.Euler(new Vector3(0, 0, 0)));
    }


}
