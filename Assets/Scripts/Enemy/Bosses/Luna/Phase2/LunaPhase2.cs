using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaPhase2 : BossBehaviourBase
{
    bool isInvulnerable = false;
    int shieldHealth = 200;
    public bool isDashing = false;

    private Vector2 dashTarget;
    private bool isBasicAttack;
    public float dashSpeed;
    private bool performAttackSkill;



    private bool homingProjectile;
    private bool grenadeProjectile;
    private bool smokeBomb;

    private Coroutine currentAttackCoroutine;

    [Header("Basic Attack")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private float fireRate = 0.2f;              // seconds between bullets
    [SerializeField] private float reloadTime = 2.0f;
    [SerializeField] private GameObject BulletPrefab;
    private int currentBullets;
    private float basicFireTimer;
    private bool isReloading;
    private float reloadTimer;


    [Header("Attack1 Trigger")]
    [SerializeField] private float homingWeight = 0.5f;
    [SerializeField] private float grenadeWeight = 0.5f;
    [SerializeField] private GameObject homingProjectilePrefab;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private GameObject grenadeWarningPrefab;

    [Header("Smoke Bomb")]
    [SerializeField] private GameObject smokeBombPrefab;
    [SerializeField] private float smokeDuration = 3f;
    [SerializeField] private float smokeCooldown = 6f;
    [SerializeField] private float burstWindowSeconds = 1f;
    [SerializeField] private float burstThresholdPercent = 0.10f; 
    private readonly Queue<(float t, int dmg)> recentDamage = new();
    private float nextSmokeTime;
    private bool smokeActive;

    [Header("Interrupt")]
    [SerializeField] private bool interruptibleByPlayerHit = true;
    [SerializeField] private float staggerDuration = 1.2f;
    private bool isStaggered;
    private float staggerTimer;

    [Header("Attack2 ")]
    [SerializeField] private float attack2DashDistance = 5f;
    [SerializeField] private float attack2DashSpeed = 18f;
    [SerializeField] private GameObject GhostPrefab;
    [SerializeField] private int ghostCount = 3;
    [SerializeField] private float ghostLifetime = 10f;
    [SerializeField] private float ghostHpPercentOfBoss = 0.05f;
    private bool untargetableDuringDash;
    private bool attack2Dashing;
    private Vector2 attack2DashTarget;


    [Header("Attack3")]
    [SerializeField] private GameObject attack3ClonePrefab;      // visual/ghost clone prefab
    [SerializeField] private GameObject fastBulletPrefab;        // fast bullet
    [SerializeField] private float attack3CloneOffset = 0.8f;
    [SerializeField] private float attack3FollowDelay = 0.08f;
    [SerializeField] private float attack3TeleportGap = 0.20f;
    [SerializeField] private float attack3PerpMinDistance = 2f;
    [SerializeField] private float attack3PerpMaxDistance = 6f;
    [Header("Attack3")]
    [SerializeField] private float attack3KnifeTeleportDistance = 1.2f; // in front of player
    [SerializeField] private float attack3KnifeRange = 1.8f;
    [SerializeField] private int attack3KnifeDamage = 120;
    [SerializeField] private float attack3DebuffDuration = 3f;
    [SerializeField] private float attack3WeakenPercent = 0.25f;
    [SerializeField] private float attack3SlowPercent = 0.3f;

    [Header("Attack4")]
    [SerializeField] private float attack4Radius = 7f;
    [SerializeField] private int attack4Laps = 3;
    [SerializeField] private float attack4AngularSpeedDeg = 180f; // orbit speed
    [SerializeField] private float attack4FireInterval = 0.18f;   // continuous fire
    [SerializeField] private float attack4AccelMultiplier = 1.5f; // "acceleration effect"
    [SerializeField] private GameObject attack4BulletPrefab;
    float attack4timesinceLastFire;
    bool circle = false;
    bool isCircling = false;
    Vector3 center;
    float smokebombTimer = 0f;

    float fireTimer = 0f;

    private enum MoveMode
    {
        None,
        DashLinear,
        CircleAroundPoint
    }
    private MoveMode moveMode = MoveMode.None;

    private Vector2 moveTarget;
    private float moveSpeed;
    private bool moveArrived;

    private Vector2 circleCenter;
    private float circleRadius;
    private float circleAngularSpeedRad;
    private float circleAngle;
    private int circleLapsTarget;
    private int circleLapsDone;

    private void Awake()
    {
        cooldown_time = 4f;
        attackProbalities = new float[5] { 20f, 20f, 20f, 20f, 20f };
        default_enemy_state = new Boss_Cooldown(1f);
        if (baseEnemyAttributes != null)
        {
            enemyAttributes = Instantiate(baseEnemyAttributes);
            health = Mathf.Max(1, enemyAttributes.maxHitPoints);
            speed = enemyAttributes.speed;
        }
        currentBullets = magazineSize;
        basicFireTimer = 0f;
        reloadTimer = 0f;
    }

    public override void BossUpdate()
    {
        current_enemy_state?.UpdateState(this);
        if (isStaggered)
        {
            staggerTimer -= Time.deltaTime;
            if (staggerTimer <= 0f) isStaggered = false;
            return;
        }

        if (!isAttacking)
            RunBasicAttack();
    }

    private bool chooseAttack1()
    {

            float choice = UnityEngine.Random.Range(0f, 1f);
            if( choice <= 0.5f)
            {
                homingProjectile = true;
                grenadeProjectile = false;
                smokeBomb = false;
            }
            else
            {
                homingProjectile= false;
                grenadeProjectile = true;
                smokeBomb = false;
            }
                return true;
        

    }


    public override void BossFixedUpdate()
    {
        if (rb == null || isStaggered) return;
        switch (moveMode)
        {
            case MoveMode.DashLinear:
                {
                    Vector2 p = rb.position;
                    Vector2 to = moveTarget - p;
                    float dist = to.magnitude;
                    float step = moveSpeed * Time.fixedDeltaTime;
                    if (dist <= step || dist <= 0.05f)
                    {
                        rb.MovePosition(moveTarget);
                        moveArrived = true;
                        moveMode = MoveMode.None;
                    }
                    else
                    {
                        rb.MovePosition(p + to.normalized * step);
                    }
                    break;
                }
            case MoveMode.CircleAroundPoint:
                {
                    // Keep player as moving center
                    if (PlayerController.instance != null)
                        circleCenter = PlayerController.instance.transform.position;
                    circleAngle += circleAngularSpeedRad * Time.fixedDeltaTime;
                    if (circleAngle >= Mathf.PI * 2f)
                    {
                        circleAngle -= Mathf.PI * 2f;
                        circleLapsDone++;
                        if (circleLapsDone >= circleLapsTarget)
                        {
                            moveMode = MoveMode.None;
                            break;
                        }
                    }
                    Vector2 next = circleCenter + new Vector2(Mathf.Cos(circleAngle), Mathf.Sin(circleAngle)) * circleRadius;
                    rb.MovePosition(next);
                    break;
                }
        }

    }


    private void checkMeleeRange()
    {

    }

    public override void Movement()
    {
        // do Basic Attack Instead
        RunBasicAttack();
    }

    private void RunBasicAttack()
    {
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                isReloading = false;
                currentBullets = magazineSize;
            }
            return;
        }
        basicFireTimer -= Time.deltaTime;
        if (basicFireTimer > 0f) return;
        basicFireTimer = fireRate;
        // fire a basic bullet
        if (BulletPrefab != null)
            Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        currentBullets--;
        if (currentBullets <= 0)
        {
            isReloading = true;
            reloadTimer = reloadTime;
        }
    }


    #region Attack1
    public override void Attack1()
    {
        if (currentAttackCoroutine != null) StopCoroutine(currentAttackCoroutine);
        currentAttackCoroutine = StartCoroutine(Attack1Coroutine());
    }
    private IEnumerator Attack1Coroutine()
    {
        isAttacking = true;
        TriggerAttack1MiniSkill();
        yield return new WaitForSeconds(0.35f); // tiny skill lock
        isAttacking = false;
        currentAttackCoroutine = null;
    }



    private void TriggerAttack1MiniSkill()
    {
        float total = Mathf.Max(0.0001f, homingWeight + grenadeWeight);
        float roll = UnityEngine.Random.value * total;
        if (roll <= homingWeight)
        {
            if (homingProjectilePrefab != null)
                Instantiate(homingProjectilePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            // throw 3 grenades near player's current position
            if (grenadePrefab == null || PlayerController.instance == null) return;
            Vector2 playerPos = PlayerController.instance.transform.position;
            for (int i = 0; i < 3; i++)
            {
                float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                float radius = UnityEngine.Random.Range(1.5f, 3.0f);
                Vector2 targetPos = playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                GameObject marker = null;
                if (grenadeWarningPrefab != null)
                    marker = Instantiate(grenadeWarningPrefab, targetPos, Quaternion.identity);
                GameObject g = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
                var grenade = g.GetComponent<Grenade>();
                if (grenade != null)
                    grenade.Throw(targetPos, marker);
            }
        }
    }
    #endregion

    #region Attack2
    public override void Attack2()
    {
        if (currentAttackCoroutine != null) StopCoroutine(currentAttackCoroutine);
        currentAttackCoroutine = StartCoroutine(Attack2Coroutine());
    }
    private IEnumerator Attack2Coroutine()
    {
        isAttacking = true;
        untargetableDuringDash = true;
        moveArrived = false;
        SpawnAttack2Ghosts();
        Vector2 dir = transform.up;
        if (PlayerController.instance != null)
        {
            Vector2 toPlayer = (Vector2)PlayerController.instance.transform.position - rb.position;
            if (toPlayer.sqrMagnitude > 0.0001f) dir = toPlayer.normalized;
        }
        moveTarget = rb.position + dir * attack2DashDistance;
        moveSpeed = attack2DashSpeed;
        moveMode = MoveMode.DashLinear;
        yield return new WaitUntil(() => moveArrived);
        untargetableDuringDash = false;
        isAttacking = false;
        currentAttackCoroutine = null;
    }

    private void SpawnAttack2Ghosts()
    {
        if (GhostPrefab == null) return;
        if (enemyAttributes == null) return;
        float spread = 0.7f;
        for (int i = 0; i < ghostCount; i++)
        {
            Vector2 offset = UnityEngine.Random.insideUnitCircle * spread;
            GameObject g = Instantiate(GhostPrefab, (Vector2)transform.position + offset, Quaternion.identity);
            var ghostEnemy = g.GetComponent<StandardEnemyBase>();
            if (ghostEnemy != null)
            {
                // 5% of Luna max hp
                int hp = Mathf.Max(1, Mathf.RoundToInt(enemyAttributes.maxHitPoints * ghostHpPercentOfBoss));
                ghostEnemy.health = hp;
                ghostEnemy.givesRewards = false;
                ghostEnemy.spawner = null;
            }
            // If your ghost script supports damage type filtering, keep this call.
            var ghostScript = g.GetComponent<LunaGhostBehaviour>();
            if (ghostScript != null)
            {
                // Optional if you add it:
                // ghostScript.SetSpiritualOnlyMode(true);
            }
            // hard lifetime cap
            Destroy(g, ghostLifetime + 0.2f);
        }
    }
    #endregion

    #region Attack 3
    public override void Attack3()
    {
        if (currentAttackCoroutine != null) StopCoroutine(currentAttackCoroutine);
        currentAttackCoroutine = StartCoroutine(Attack3Coroutine());
    }

    private IEnumerator Attack3Coroutine()
    {
        isAttacking = true;
        if (PlayerController.instance == null)
        {
            isAttacking = false;
            currentAttackCoroutine = null;
            yield break;
        }
        Transform player = PlayerController.instance.transform;

        GameObject c1 = null, c2 = null;
        if (attack3ClonePrefab != null)
        {
            c1 = Instantiate(attack3ClonePrefab, transform.position + Vector3.left * attack3CloneOffset, Quaternion.identity);
            c2 = Instantiate(attack3ClonePrefab, transform.position + Vector3.right * attack3CloneOffset, Quaternion.identity);
        }

        for (int round = 0; round < 2; round++)
        {
            Vector2 p1 = PickPerpendicularPointAroundPlayer(player.position, true);
            Vector2 p2 = PickPerpendicularPointAroundPlayer(player.position, false);
            Vector2 po = PickPerpendicularPointAroundPlayer(player.position, UnityEngine.Random.value > 0.5f);

            TeleportAndShoot(c1, p1);
            TeleportAndShoot(c2, p2);
            yield return new WaitForSeconds(attack3FollowDelay);

            TeleportAndShoot(gameObject, po);
            yield return new WaitForSeconds(attack3TeleportGap);
        }

        if (c1 != null) Destroy(c1);
        if (c2 != null) Destroy(c2);

        Vector2 frontPos = GetPointInFrontOfPlayer(player, attack3KnifeTeleportDistance);
        rb.position = frontPos; // instant teleport is OK
        DoKnifeStrike(player);
        isAttacking = false;
        currentAttackCoroutine = null;
    }

    private Vector2 PickPerpendicularPointAroundPlayer(Vector2 playerPos, bool positiveSide)
    {
        Vector2 aim = GetPlayerAimDirectionSafe();
        Vector2 perp = new Vector2(-aim.y, aim.x).normalized;
        if (!positiveSide) perp = -perp;
        float d = UnityEngine.Random.Range(attack3PerpMinDistance, attack3PerpMaxDistance);
        Vector2 candidate = playerPos + perp * d;
        // keep inside map if helper exists
        if (GameManager.instance != null && !GameManager.instance.IsWorldPointInsideMap(candidate, 0f))
            candidate = playerPos + perp * attack3PerpMinDistance;
        return candidate;
    }
    private Vector2 GetPlayerAimDirectionSafe()
    {

        Vector2 dir = Vector2.right;
        if (PlayerController.instance != null)
        {
            dir = PlayerController.instance.transform.right;
            if (dir.sqrMagnitude < 0.0001f)
                dir = PlayerController.instance.transform.up;
        }
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        return dir.normalized;
    }
    private void TeleportAndShoot(GameObject actor, Vector2 toPos)
    {
        if (actor == null) return;
        var tr = actor.transform;
        tr.position = new Vector3(toPos.x, toPos.y, tr.position.z);
        if (fastBulletPrefab != null)
            Instantiate(fastBulletPrefab, tr.position, Quaternion.identity);
    }
    private Vector2 GetPointInFrontOfPlayer(Transform player, float dist)
    {
        Vector2 aim = GetPlayerAimDirectionSafe();
        return (Vector2)player.position + aim * dist;
    }
    private void DoKnifeStrike(Transform player)
    {
        if (Vector2.Distance(transform.position, player.position) > attack3KnifeRange)
            return;
        PlayerController.instance.TakeDamage(
            attack3KnifeDamage,
            DamageInstance.DamageSource.Enemy,
            DamageInstance.DamageType.Physical);
        var em = PlayerController.instance.GetComponent<PlayerEffectManager>();
        if (em != null && PlayerController.instance.playerAttributes != null)
        {
            em.AddEffect(new Weak_Effect(attack3DebuffDuration, attack3WeakenPercent), PlayerController.instance.playerAttributes);
            em.AddEffect(new Slow_Effect(attack3DebuffDuration, attack3SlowPercent), PlayerController.instance.playerAttributes);
        }
    }

    #endregion

    #region Attack 4
    public override void Attack4()
    {
        if (currentAttackCoroutine != null) StopCoroutine(currentAttackCoroutine);
        currentAttackCoroutine = StartCoroutine(Attack4Coroutine());
    }
    private IEnumerator Attack4Coroutine()
    {
        isAttacking = true;
        if (PlayerController.instance == null || rb == null)
        {
            isAttacking = false;
            currentAttackCoroutine = null;
            yield break;
        }
        // acceleration effect
        float oldSpeed = speed;
        speed *= attack4AccelMultiplier;
        // lock orbit center at cast start (simple and stable)
        circleCenter = PlayerController.instance.transform.position;
        circleRadius = Mathf.Max(1f, attack4Radius);
        circleLapsTarget = Mathf.Max(1, attack4Laps);
        circleLapsDone = 0;
        // start angle from current position around center
        Vector2 fromCenter = rb.position - circleCenter;
        if (fromCenter.sqrMagnitude < 0.0001f) fromCenter = Vector2.right * circleRadius;
        circleAngle = Mathf.Atan2(fromCenter.y, fromCenter.x);
        // convert deg/s to rad/s and apply acceleration multiplier
        circleAngularSpeedRad = Mathf.Deg2Rad * attack4AngularSpeedDeg * attack4AccelMultiplier;
        moveMode = MoveMode.CircleAroundPoint;
        float fireTimer = 0f;
        while (moveMode == MoveMode.CircleAroundPoint)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                FireAttack4BulletTowardPlayer();
                fireTimer = attack4FireInterval;
            }
            yield return null;
        }
        // restore
        speed = oldSpeed;
        isAttacking = false;
        currentAttackCoroutine = null;
    }

    private void FireAttack4BulletTowardPlayer()
    {
        if (attack4BulletPrefab == null || PlayerController.instance == null) return;
        Vector2 origin = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 toPlayer = ((Vector2)PlayerController.instance.transform.position - origin).normalized;
        float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        // spawn facing player
        Instantiate(attack4BulletPrefab, origin, Quaternion.Euler(0f, 0f, angle));
    }

    #endregion
    public override void Attack5()
    {
        isAttacking = true;
        dashTarget = PlayerController.instance.transform.position;

        isDashing = true;

    }


   

    

    #region Smoke Bomb and Interrupt Logic

    public override void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        if (untargetableDuringDash && dmgSource == DamageInstance.DamageSource.Player)
        {
            ShowFloatingText(0, dmgType);
            return;
        }
        if (enemyAttributes == null) return;
        int damageAfterReduction = enemyAttributes.DamageCalculation(amount, DamageInstance.ToEnemyDamageType(dmgType));
        // regular damage
        health -= damageAfterReduction;
        ShowFloatingText(damageAfterReduction, dmgType);
        // record burst window for smoke trigger
        RegisterRecentDamage(damageAfterReduction);
        TryTriggerSmokeBomb();
        // interrupt skill if hit by player while casting
        if (interruptibleByPlayerHit &&
            dmgSource == DamageInstance.DamageSource.Player &&
            isAttacking)
        {
            InterruptCurrentSkill();
        }
        if (health <= 0) Die();
    }
    private void RegisterRecentDamage(int dmg)
    {
        float now = Time.time;
        recentDamage.Enqueue((now, dmg));
        while (recentDamage.Count > 0 && now - recentDamage.Peek().t > burstWindowSeconds)
            recentDamage.Dequeue();
    }
    private void TryTriggerSmokeBomb()
    {
        if (smokeActive) return;
        if (Time.time < nextSmokeTime) return;
        if (enemyAttributes == null) return;
        int total = 0;
        foreach (var e in recentDamage) total += e.dmg;
        float threshold = enemyAttributes.maxHitPoints * burstThresholdPercent;
        if (total >= threshold)
        {
            StartCoroutine(SmokeBombRoutine());
            nextSmokeTime = Time.time + smokeCooldown;
        }
    }
    private IEnumerator SmokeBombRoutine()
    {
        smokeActive = true;
        if (smokeBombPrefab != null)
            Instantiate(smokeBombPrefab, transform.position, Quaternion.identity);
        // minimal "hide" effect: reduce alpha
        var sr = GetComponent<SpriteRenderer>();
        Color old = Color.white;
        if (sr != null)
        {
            old = sr.color;
            sr.color = new Color(old.r, old.g, old.b, 0.35f);
        }
        yield return new WaitForSeconds(smokeDuration);
        if (sr != null) sr.color = old;
        smokeActive = false;
    }
    private void InterruptCurrentSkill()
    {
        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
            currentAttackCoroutine = null;
        }
        isDashing = false;
        isAttacking = false;
        isStaggered = true;
        staggerTimer = staggerDuration;
    }

    #endregion




}
