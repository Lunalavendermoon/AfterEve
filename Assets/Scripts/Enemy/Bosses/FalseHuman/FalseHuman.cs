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

    private void Awake()
    {
        cooldown_time = 3f;
        default_enemy_state = new Boss_Attack(5);
        attackProbalities = new float[5] { 25.0f, 25.0f, 25.0f, 25.0f,0f };
        //Debug.Log("Boss Start");

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
        //Summons a Knight of Blades, a Knight of Shields, and a Knight of Hammers(see enemy sheet).
        //After finishing this attack, waits 5 seconds before using another attack.
        isAttacking = true;
        attackProbalities = new float[5] { 33.3f, 0.0f, 33.30f, 33.30f, 0f };
        SpawnKnights();

        cooldown_time = 5f;
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
        //Debug.Log("Attack 3: Emotion Capsules deployed.");
        attackProbalities = new float[5] { 33.30f, 33.3f, 0f, 33.30f, 0f };
        for (int i = 0; i < 3; i++)
        {
            Vector2 random = Random.insideUnitCircle;


            Instantiate(emotionCapsule, new Vector3(random.x*3f,random.y*3f, transform.position.z), Quaternion.Euler(new Vector3(0, 0, 0)));
        }
        cooldown_time = 7f;
        isAttacking = false;
    }
    public override void Attack4()
    {
        isAttacking = true;
        //Shoots out a large projectile with 1000 init health.
        //The projectile follows the player at a speed of 1 for ten seconds, then the projectile permanant gain 400% Haste buff.
        //If not destroyed by the time it hits the player, it deals the remaining projectile health as spiritual dmg to the player.
        Instantiate(largeProjectile, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
        //Debug.Log("Large projectile launched.");
        attackProbalities = new float[5] { 33.30f, 33.3f, 33.30f, 0f, 0f };
        cooldown_time = 10f;
        isAttacking = false;
    }

    public override void Attack5()
    {
        //Gain a shield at the beginning of the battle preventing
        //all physical dmg to the boss that blocks 200 spiritual damage and has the same defense as the boss. 
        //The boss does not use this skill other than once at the start of the battle.
        Debug.Log("attack 5");
        isInvulnerable = true;
        cooldown_time = 5f;
    }


    public override void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        int damageAfterReduction = Mathf.CeilToInt(amount * (1 - (enemyAttributes.basicDefense / (enemyAttributes.basicDefense + 100))));
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
            Debug.Log($"{gameObject.name} is invulnerable and took no damage.");
            ShowFloatingText(0);

            return;
        }
        health -= damageAfterReduction;

        //OnEnemyDamageTaken?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);

        // Damage numbers
        ShowFloatingText(damageAfterReduction);
        Debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
        if (health <= 0)
        {
            // TODO: set hitWeakPoint to true/false depending on whether weak point was hit with the current attack
            //OnEnemyDeath?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);
            Die();
        }
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
                Quaternion.Euler(new Vector3(0, 0, 0)
            ));

            EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
            enemy.chest = null;
            enemy.spawner = null;
            enemy.givesRewards = false;
        }
    }

    IEnumerator Attack1_Coroutine()
    {
        isAttacking = true;

        Debug.Log("Attack 1: Shooting projectiles.");
        int[] projectileCounts = new int[] { 5,7,9};
        for (int round = 0; round < projectileCounts.Length; round++)
        {

            Debug.Log($"Attack 1: Round {round + 1} - Shooting {projectileCounts[round]} projectiles.");
            int count = projectileCounts[round];
            float angleStep = 360f / count;
            float angleOffset = Random.Range(0f, angleStep);
            for (int i = 0; i < count; i++)
            {
                GameObject target = new GameObject("ProjectileTarget");
                target.transform.position = PlayerController.instance.transform.position;
                float angle = angleOffset + i * angleStep;
                
                Vector3 spawnpoint = new Vector3(transform.position.x+Mathf.Cos(angle * Mathf.Deg2Rad),
                                                 transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad), 
                                                 transform.position.z);
                GameObject projectile = Instantiate(projectilePrefab, spawnpoint, Quaternion.identity);
                BossProjectileScript projectileScript = projectile.GetComponent<BossProjectileScript>();
                projectileScript.InitializeProjectile(
                    target,
                    projectileMaxMoveSpeed,
                    projectileMaxHeight

                );
                projectileScript.InitializeAnimationCurves(
                    trajectoryAnimationCurve,
                    axisCorrectionAnimationCurve,
                    projectileSpeedAnimationCurve
                    );
                
            }
            Debug.Log($"Attack 1: Round {round + 1} complete. Waiting 2 seconds before next round.");
            yield return new WaitForSeconds(2f);
        }
        isAttacking = false;
    }
}
