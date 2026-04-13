using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemySpawnerScript;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class FalseHuman : BossBehaviourBase
{
    bool isInvulnerable = false;
    int shieldHealth = 200;

    [SerializeField] private float projectileMaxMoveSpeed;
    [SerializeField] private float projectileMaxHeight;
    [SerializeField] private GameObject projectilePrefab;


    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;

    private float shootTimer;

    public LargeProjectile largeProjectile;
    public List<EnemyEntry> knightsList = new();
    public EmotionCapsule emotionCapsule;
    private const float EmotionWaveDelay = 5f;
    private const int EmotionWaveDamageToPlayer = 600;
    private const int EmotionWaveDamageToBossPerCapsule = 1000;
    private const float EmotionShieldRadius = 2f;
    private const float EmotionDebuffDuration = 7f;
    private const float EmotionWeakPercent = 0.2f;

    [Header("Attack 2 — knight summons")]
    [SerializeField] float knightSummonRandomRadius = 4f;

    [Header("Attack 4 — large projectile")]
    [Tooltip("World-units in front of the boss along the aim line toward the player.")]
    [SerializeField] float largeProjectileSpawnForwardOffset = 1.25f;

    private void Awake()
    {
        if (baseEnemyAttributes != null)
        {
            enemyAttributes = Instantiate(baseEnemyAttributes);
            health = Mathf.Max(1, enemyAttributes.maxHitPoints);
            speed = enemyAttributes.speed;
        }
        else
        {
            health = Mathf.Max(1, health);
        }
        cooldown_time = 3f;
        default_enemy_state = new Boss_Attack(5);
        attackProbalities = new float[5] { 25.0f, 25.0f, 25.0f, 25.0f, 0f };
    }
    public override void Movement()
    {
        // move towards player
        Pathfinding(PlayerController.instance.transform);
        
    }
    public override void Attack1()
    {
        //Debug.Log("attack 1 Start");
        //Shoots 3 rounds of projectiles, consisting 5, 7, and 9 projectiles respectivly,
        //each with randomized path using Cubic Bezier curve (set the init player position as the destination).
        //Each dealing 30 spiritual dmg. 2 seconds between rounds.
        //After finishing this attack, waits 3 seconds before using another attack.
        attackProbalities = new float[5] { 0.0f, 33.3f, 33.30f, 33.30f, 0f };
        StartCoroutine(Attack1_Coroutine());
        cooldown_time = 3f;
    }
    public override void Attack2()
    {
        StartCoroutine(SummonKnightsAttack());
    }
    IEnumerator SummonKnightsAttack()
    {
        isAttacking = true;
        attackProbalities = new float[5] { 33.3f, 0.0f, 33.30f, 33.30f, 0f };
        foreach (EnemyEntry entry in knightsList)
        {
            if (entry.enemyPrefab == null)
            {
                Debug.LogWarning("FalseHuman knightsList entry missing enemyPrefab.");
                continue;
            }
            BossSummonUtility.SpawnBossMinionFromEntry(transform.position, knightSummonRandomRadius, entry);
        }
        cooldown_time = 5f;
        yield return null;
        isAttacking = false;
    }
    public override void Attack3()
    {
        //Shoots out 3 emotion capsules in random position on the map,
        //capsules stay stationary on the map
        //and can be activated into a stationary shield of 2 units radius
        //upon recieving spiritual damage from the player.
        //After 5 seconds send out a emotion shock wave that deal 600 spiritual dmg
        //and applies a Blindness and 20% Weak debuff for 7 seconds to the player
        //if the player is not inside a activated shield.
        //After the wave, each activated capsule deals 1000 dmg to the boss.
        //After finishing this attack, waits 7 seconds before using another attack.
        isAttacking = true;
        attackProbalities = new float[5] { 33.30f, 33.3f, 0f, 33.30f, 0f };
        var capsules = new List<EmotionCapsule>();
        for (int i = 0; i < 3; i++)
        {
            Vector2 random = Random.insideUnitCircle;
            GameObject go = Instantiate(emotionCapsule.gameObject, new Vector3(random.x * 3f, random.y * 3f, transform.position.z), Quaternion.identity);
            EmotionCapsule cap = go.GetComponent<EmotionCapsule>();
            if (cap != null) capsules.Add(cap);
        }
        StartCoroutine(Attack3_WaveCoroutine(capsules));
    }
    public override void Attack4()
    {
        isAttacking = true;
        //Shoots out a large projectile with 1000 init health.
        //The projectile follows the player at a speed of 1 for ten seconds, then the projectile permanant gain 400% Haste buff.
        //If not destroyed by the time it hits the player, it deals the remaining projectile health as spiritual dmg to the player.
        Instantiate(largeProjectile, GetLargeProjectileSpawnPosition(), Quaternion.Euler(new Vector3(0, 0, 0)));
        //Debug.Log("Large projectile launched.");
        attackProbalities = new float[5] { 33.30f, 33.3f, 33.30f, 0f, 0f };
        cooldown_time = 10f;
        isAttacking = false;
    }
    Vector3 GetLargeProjectileSpawnPosition()
    {
        Vector3 origin = transform.position;
        Vector2 dir = Vector2.right;
        if (PlayerController.instance != null)
        {
            dir = (Vector2)PlayerController.instance.transform.position - (Vector2)origin;
            if (dir.sqrMagnitude > 1e-6f)
                dir.Normalize();
        }
        Vector3 offset = new Vector3(dir.x, dir.y, 0f) * largeProjectileSpawnForwardOffset;
        return origin + offset;
    }

    public override void Attack5()
    {
        //Gain a shield at the beginning of the battle preventing
        //all physical dmg to the boss that blocks 200 spiritual damage and has the same defense as the boss. 
        //The boss does not use this skill other than once at the start of the battle.
        isInvulnerable = true;
        cooldown_time = 5f;
        isAttacking = false;
    }


    public override void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        if (enemyAttributes == null)
            return;
        int damageAfterReduction;
        if (dmgType == DamageInstance.DamageType.Spiritual)
            damageAfterReduction = Mathf.CeilToInt(amount * (1f - (enemyAttributes.spiritualDefense / (enemyAttributes.spiritualDefense + 100f))));
        else
            damageAfterReduction = Mathf.CeilToInt(amount * (1f - (enemyAttributes.basicDefense / (enemyAttributes.basicDefense + 100f))));
        if (isInvulnerable)
        {
            if (dmgType == DamageInstance.DamageType.Spiritual)
            {
                shieldHealth -= damageAfterReduction;
                ShowFloatingText(damageAfterReduction);
                if (shieldHealth <= 0)
                {
                    isInvulnerable = false;
                    Debug.Log($"{gameObject.name}'s shield has been broken!");
                }
                return;
            }
            ShowFloatingText(0);
            return;
        }
        health -= damageAfterReduction;
        ShowFloatingText(damageAfterReduction);
        Debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
        if (health <= 0)
            Die();
    }


    void SpawnKnights()
    {
        foreach (var entry in knightsList)
        {
            if (entry.enemyPrefab == null || entry.spawnPoint == null)
            {
                Debug.LogWarning("EnemyEntry has missing prefab or spawn point.");
                continue;
            }
            GameObject enemyObj = Instantiate(
                entry.enemyPrefab,
                entry.spawnPoint.position,
                Quaternion.Euler(new Vector3(0, 0, 0)));
            StandardEnemyBase enemy = enemyObj.GetComponent<StandardEnemyBase>();
            if (enemy != null)
            {
                enemy.chest = null;
                enemy.spawner = null;
                enemy.givesRewards = false;
            }
        }
    }

    IEnumerator Attack1_Coroutine()
    {
        isAttacking = true;
        int[] projectileCounts = new int[] { 5, 7, 9 };
        for (int round = 0; round < projectileCounts.Length; round++)
        {
            Vector3 roundTargetPosition = PlayerController.instance != null
                ? PlayerController.instance.transform.position
                : transform.position + Vector3.down * 5f;
            int count = projectileCounts[round];
            float angleStep = 360f / count;
            float angleOffset = Random.Range(0f, angleStep);
            for (int i = 0; i < count; i++)
            {
                GameObject target = new GameObject("ProjectileTarget");
                target.transform.position = roundTargetPosition;
                float angle = angleOffset + i * angleStep;
                Vector3 spawnpoint = new Vector3(
                    transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad),
                    transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad),
                    transform.position.z);
                GameObject projectile = Instantiate(projectilePrefab, spawnpoint, Quaternion.identity);
                BossProjectileBezier projectileScript = projectile.GetComponent<BossProjectileBezier>();
                if (projectileScript != null)
                {
                    projectileScript.InitializeProjectile(target, projectileMaxMoveSpeed, projectileMaxHeight);
                    //projectileScript.InitializeAnimationCurves(
                    //    trajectoryAnimationCurve,
                    //    axisCorrectionAnimationCurve,
                    //    projectileSpeedAnimationCurve);
                }
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(2f);
        }
        isAttacking = false;
    }

    private IEnumerator Attack3_WaveCoroutine(List<EmotionCapsule> capsules)
    {
        yield return new WaitForSeconds(EmotionWaveDelay);
        bool playerShielded = false;
        if (PlayerController.instance != null)
        {
            Vector3 playerPos = PlayerController.instance.transform.position;
            foreach (var cap in capsules)
            {
                if (cap != null && cap.IsPositionInsideShield(playerPos))
                {
                    playerShielded = true;
                    break;
                }
            }
        }
        int totalCapsules = capsules.Count;
        if (totalCapsules <= 0) totalCapsules = 1;
        float damagePerCapsule = (float)EmotionWaveDamageToPlayer / totalCapsules;
        int unactivatedCount = 0;
        foreach (var cap in capsules)
        {
            if (cap != null && !cap.IsActivated)
                unactivatedCount++;
        }

        if (!playerShielded && PlayerController.instance != null)
        {
            int damageToDeal = Mathf.RoundToInt(damagePerCapsule * unactivatedCount);
            if (damageToDeal > 0)
            {
                PlayerController.instance.TakeDamage(damageToDeal, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Spiritual);
                var effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();
                if (effectManager != null && PlayerController.instance.playerAttributes != null)
                {
                    effectManager.AddEffect(new Blindness_Effect(EmotionDebuffDuration), PlayerController.instance.playerAttributes);
                    effectManager.AddEffect(new Weak_Effect(EmotionDebuffDuration, EmotionWeakPercent), PlayerController.instance.playerAttributes);
                }
            }
        }
        foreach (var cap in capsules)
        {
            if (cap != null && cap.IsActivated)
                TakeDamage(EmotionWaveDamageToBossPerCapsule, DamageInstance.DamageSource.Environment, DamageInstance.DamageType.Spiritual);
        }
        foreach (var cap in capsules)
        {
            if (cap != null) Destroy(cap.gameObject);
        }
        isAttacking = false;
    }

    public override bool ShouldBlockEffect(Effects effect)
    {
        return effect.effectStat == Effects.Stat.Movement
            || effect.effectStat == Effects.Stat.SpiritualVision
            || effect.effectStat == Effects.Stat.Confused
            || effect.effectStat == Effects.Stat.Knockback;
    }
}
