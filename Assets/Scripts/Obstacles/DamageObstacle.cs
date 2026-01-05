using UnityEngine;

public class DamageObstacle : MonoBehaviour
{
    public int baseDamage = 2; //arbitrary???

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(baseDamage, DamageInstance.DamageSource.Environment, DamageInstance.DamageType.Basic);
            return;
        }

        // Enemy
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(baseDamage, DamageInstance.DamageSource.Environment, DamageInstance.DamageType.Basic);
        }
    }
}
