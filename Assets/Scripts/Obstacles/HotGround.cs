using UnityEngine;

public class HotGround : MonoBehaviour
{
    [Header("Burn Settings")]
    public float burnDuration = 2.0f;
    public int burnDamagePerTick = 1;
    public float tickInterval = 0.5f;

    private void OnTriggerStay2D(Collider2D other)
    {
        // Player
        PlayerEffectManager playerEffects = other.GetComponent<PlayerEffectManager>();

        if (playerEffects != null)
        {
            ApplyBurn(playerEffects, PlayerController.instance.playerAttributes);
            return;
        }

        // Enemy
        EnemyEffectManager enemyEffects = other.GetComponent<EnemyEffectManager>();
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        if (enemyEffects != null)
        {
            ApplyBurn(enemyEffects, enemy.enemyAttributes);
        }
    }

    private void ApplyBurn(EffectManager effectManager, EntityAttributes attr)
    {
        // Reapply burn continuously while standing on hot ground
        Burn_Effect burn = new Burn_Effect(burnDuration, burnDamagePerTick, tickInterval);
        effectManager.AddEffect(burn, attr);
    }
}
