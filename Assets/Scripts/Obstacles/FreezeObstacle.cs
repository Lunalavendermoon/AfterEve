using UnityEngine;

public class FreezeObstacle : MonoBehaviour
{
    public float freezeDuration = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player
        PlayerEffectManager playerEffects = other.GetComponent<PlayerEffectManager>();

        if (playerEffects != null)
        {
            playerEffects.AddEffect(new Paralyze_Effect(freezeDuration));
            return;
        }

        // Enemy
        EnemyEffectManager enemyEffects = other.GetComponent<EnemyEffectManager>();

        if (enemyEffects != null)
        {
            enemyEffects.AddEffect(new Paralyze_Effect(freezeDuration));
        }
    }
}
