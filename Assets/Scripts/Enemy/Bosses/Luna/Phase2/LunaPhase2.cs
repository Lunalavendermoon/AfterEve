using System;
using System.Collections;
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


    [Header("Basic Attack Stats")]
    [SerializeField] private int magazineSize;
    [SerializeField] private int currentBullets;
    [SerializeField] private float reloadTime;
    [SerializeField] private float teleportChance;
    [SerializeField] private float fireRate;
    private float timeSinceLastFire;
    [Tooltip("Attack 1 chance")]
    [SerializeField] private float attack1Chance;

    [SerializeField] private GameObject homingProjectilePrefab;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private GameObject grenadeWarningPrefab;
    [SerializeField] private GameObject GhostPrefab;

    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private int attack5GrenadeNumber;

    [Header("Attack 4")]
    [SerializeField] private float attack4Radius;
    [SerializeField] private float attack4FireRate;
    float attack4timesinceLastFire;
    bool circle = false;
    bool isCircling = false;
    Vector3 center;


    float fireTimer = 0f;


    private void Awake()
    {
        cooldown_time = 4f;
        attackProbalities = new float[5] { 0.0f, 25.0f, 25.0f, 25.0f, 25.0f };
        default_enemy_state = new Boss_Cooldown(1f);



    }

    public override void BossUpdate()
    {
        current_enemy_state.UpdateState(this);
        

        if (isBasicAttack)
        {
            float dt = Time.deltaTime;
            if (chooseAttack1())
            {
                ChangeState(new Boss_Attack(1));
                timeSinceLastFire = 0f;
                return;
            }
            if (timeSinceLastFire > fireRate)
            {
                Instantiate(BulletPrefab, transform.position, Quaternion.identity);
                timeSinceLastFire = 0f;
            }
            timeSinceLastFire += dt;
            

        }
    }

    private bool chooseAttack1()
    {
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random <= attack1Chance)
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
        else
            return false;
    }


    public override void BossFixedUpdate()
    {
        if (isDashing)
        {
            performDash();
        }

    }

    private void performDash()
    {
        if (Vector2.Distance(transform.position, dashTarget) < 0.5f)
        {
            isDashing = false;
            isAttacking = false;
            return;
        }

        Vector2 currentpos = transform.position;
        Vector2 dashDirection = dashTarget - currentpos;
        Vector2 dashDelta = dashDirection * dashSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + dashDelta);
    }

    private void checkMeleeRange()
    {

    }

    public override void Movement()
    {
        // do Basic Attack Instead
        BasicAttack();
    }

    public void BasicAttack()
    {
        if (!isBasicAttack)
            isBasicAttack = true;

    }



    public override void Attack1()
    {
        StartCoroutine(Attack1Coroutine());

    }
    public override void Attack2()
    {
        //Dashes in a straight line (5 units distance) that leaves behind 3 ghostly clones of her for 10 seconds.
        //These clones only deal and take spiritual damage, but only have 5% of the boss max health.
        //They perform Attack 1 when they spawn, 5 seconds after they spawned, and 10 seconds when they spawn.
        //If destroyed before their life time, they do not perform the rest of their Attack 1.
        //Luna is untargetable during the dash and does not take direct damage
        //(Previous debuffs before she dashes continues to affect her).

        //set dashtarget to 5 units infront of its current direction

        Debug.Log("Attack2");
        SpawnGhost();

        dashTarget = transform.position + transform.up * 5f;

        isDashing = true;

        SpawnGhost();
        SpawnGhost();

    }

    private void SpawnGhost()
    {
        Instantiate(GhostPrefab, transform.position, Quaternion.identity);

    }

    public override void Attack3()
    {
        //Rapidly creates 2 clones, resulting in a total of 3 entities including the original.
        //They quickly teleport twice at the diagonal line of sight from the player,
        //firing one fast bullet after each teleport before continuing to the next location.
        //The two clones move out first, and the original body follows last (the time interval between them is very short).
        //After the teleports are complete, the clones disappear,
        //and the original body teleports in front of the player to launch a quick close-combat knife attack.
        //If hit, it deals x damage and applies a Weaken and Slow effect for x seconds to the player.


        StartCoroutine(Attack3Coroutine());

    }
    public override void Attack4()
    {
        circle = true;
        isCircling = false;
    }

    public override void Attack5()
    {
        isAttacking = true;
        dashTarget = PlayerController.instance.transform.position;

        isDashing = true;

    }


    public override void TakeDamage(int amount, DamageInstance.DamageSource dmgSource, DamageInstance.DamageType dmgType)
    {
        int damageAfterReduction = enemyAttributes.DamageCalculation(amount, DamageInstance.ToEnemyDamageType(dmgType));
        if (isInvulnerable)
        {
            if (dmgType == DamageInstance.DamageType.Spiritual)
            {
                shieldHealth -= damageAfterReduction;
                ShowFloatingText(damageAfterReduction, dmgType);
                if (shieldHealth <= 0)
                {
                    isInvulnerable = false;
                    Debug.Log($"{gameObject.name}'s shield has been broken!");
                }
                return;
            }
            Debug.Log($"{gameObject.name} is invulnerable and took no damage.");
            ShowFloatingText(0, dmgType);

            return;
        }
        health -= damageAfterReduction;

        //OnEnemyDamageTaken?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);

        // Damage numbers
        ShowFloatingText(damageAfterReduction, dmgType);
        Debug.Log($"{gameObject.name} took {amount} damage, remaining health: {health}");
        if (health <= 0)
        {
            // TODO: set hitWeakPoint to true/false depending on whether weak point was hit with the current attack
            //OnEnemyDeath?.Invoke(new DamageInstance(dmgSource, dmgType, amount, damageAfterReduction), this);
            Die();
        }
    }


    private IEnumerator Attack1Coroutine()
    {
        isAttacking = true;
        if (homingProjectile)
        {
            Instantiate(homingProjectilePrefab, transform.position, Quaternion.identity);
            homingProjectile = false;

        }
        else if (grenadeProjectile)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(grenadePrefab, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.5f);
                
            }
            grenadeProjectile = false;
        }
        else if (smokeBomb)
        {
            Instantiate(grenadePrefab, transform.position, Quaternion.identity);
            //hiding code here
        }
        isAttacking = false;


    }

    private IEnumerator Attack3Coroutine()
    {
        // generate 7 random spots around the map
        isAttacking = true;

        


        yield return new WaitForSeconds(1f);

        

        isAttacking = false;
    }

    private IEnumerator Attack4Coroutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }



}
