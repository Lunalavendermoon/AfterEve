using UnityEngine;
using System;
using System.Collections.Generic;

public class HotGround : MonoBehaviour
{
    [Header("Burn Settings")]
    public float burnDuration = 2.0f;
    public int burnDamagePerTick = 10;
    public float tickInterval = 0.5f;

    readonly Dictionary<int, float> nextTickTime = new();

    private void OnTriggerStay2D(Collider2D other)
    {
        int entityId = other.transform.root.GetInstanceID();

        // Player
        PlayerEffectManager playerEffects = other.GetComponentInParent<PlayerEffectManager>();

        if (playerEffects != null)
        {
            ApplyContinuousBurn(entityId, playerEffects);
            return;
        }

        // Enemy
        EnemyEffectManager enemyEffects = other.GetComponentInParent<EnemyEffectManager>();

        if (enemyEffects != null)
        {
            ApplyContinuousBurn(entityId, enemyEffects);
        }
    }

    private void ApplyContinuousBurn(int entityId, EffectManager effectManager)
    {
        if (effectManager == null)
        {
            return;
        }

        float now = Time.time;
        if (!nextTickTime.TryGetValue(entityId, out float nextTime) || now >= nextTime)
        {
            nextTickTime[entityId] = now + tickInterval;
            DealTickDamage(effectManager);
        }
    }

    private void DealTickDamage(EffectManager effectManager)
    {
        if (effectManager is PlayerEffectManager)
        {
            PlayerController.instance.TakeDamage(burnDamagePerTick, DamageInstance.DamageSource.Environment, DamageInstance.DamageType.Physical);
            return;
        }

        if (effectManager is EnemyEffectManager eem && eem.enemy != null)
        {
            eem.enemy.TakeDamage(burnDamagePerTick, DamageInstance.DamageSource.Environment, DamageInstance.DamageType.Physical);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        int entityId = other.transform.root.GetInstanceID();

        // Player
        PlayerEffectManager playerEffects = other.GetComponentInParent<PlayerEffectManager>();
        if (playerEffects != null)
        {
            nextTickTime.Remove(entityId);
            var player = PlayerController.FindScenePlayer();
            ApplyExitBurn(playerEffects, player != null ? player.playerAttributes : null);
            return;
        }

        // Enemy
        EnemyEffectManager enemyEffects = other.GetComponentInParent<EnemyEffectManager>();
        EnemyBase enemy = other.GetComponentInParent<EnemyBase>();
        if (enemyEffects != null)
        {
            nextTickTime.Remove(entityId);
            ApplyExitBurn(enemyEffects, enemy != null ? enemy.enemyAttributes : null);
        }
    }

    private void ApplyExitBurn(EffectManager effectManager, EntityAttributes attr)
    {
        if (effectManager == null || attr == null)
        {
            return;
        }

        Burn_Effect burn = new Burn_Effect(burnDuration, burnDamagePerTick, tickInterval);
        effectManager.AddEffect(burn, attr);
    }
}
