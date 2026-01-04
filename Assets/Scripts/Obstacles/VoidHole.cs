using UnityEngine;

public class VoidHole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // stuff falls
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            KillEnemy(enemy);
        }
    }

    private void KillEnemy(EnemyBase enemy)
    {
        // ADD THE SOUND HERE :D
        Destroy(enemy.gameObject);
    }
}
