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
            ApplyBurn(playerEffects);
            return;
        }

        // Enemy
        EnemyEffectManager enemyEffects = other.GetComponent<EnemyEffectManager>();

        if (enemyEffects != null)
        {
            ApplyBurn(enemyEffects);
        }
    }

    private void ApplyBurn(EffectManager effectManager)
    {
        // Reapply burn continuously while standing on hot ground
        Burn_Effect burn = new Burn_Effect(burnDuration, burnDamagePerTick, tickInterval);
        effectManager.AddEffect(burn);
    }
}
