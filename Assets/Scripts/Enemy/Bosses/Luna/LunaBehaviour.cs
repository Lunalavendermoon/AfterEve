using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemySpawnerScript;

public class LunaBehaviour : BossBehaviourBase
{
    bool isInvulnerable = false;
    int shieldHealth = 200;
    public bool isDashing = false;

    private Vector2 dashTarget;
    public float dashSpeed;

    
    [SerializeField] private GameObject projectilePrefab1;
    [SerializeField] private GameObject projectilePrefab3;
    [SerializeField] private GameObject projectilePrefab4;



    private float shootTimer;


    private void Awake()
    {
        cooldown_time = 4f;
        default_enemy_state = new Boss_Cooldown(1f);
        
        Debug.Log("Boss Start");

    }

    public override void BossUpdate()
    {
        current_enemy_state.UpdateState(this);
        checkMeleeRange();
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
        if(Vector2.Distance(transform.position, dashTarget) < 0.5f)
        {
            isDashing = false;
            isAttacking = false;
            return;
        }
        Debug.Log(Vector2.Distance(transform.position, dashTarget));
        Vector2 currentpos= transform.position;
        Vector2 dashDirection =dashTarget- currentpos;
        Vector2 dashDelta = dashDirection*dashSpeed*Time.fixedDeltaTime;
        rb.MovePosition(rb.position+dashDelta);
    }

    private void checkMeleeRange()
    {
        if(Vector3.Distance(PlayerController.instance.transform.position, transform.position) <= 4f)
        {

            attackProbalities = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 100f };
        }
        else
        {
            attackProbalities = new float[5] { 50.0f, 50.0f, 0.0f, 0.0f, 0f };
        }
    }

    public override void Movement()
    {
        // move towards player
        Pathfinding(PlayerController.instance.transform);
    }
    public override void Attack1()
    {
        Debug.Log("attack 1 Start");
        //Fire 4 speedy projectiles, each dealing 40 basic damage, t
        //hat bounce 2 times if it hits a wall, accelerating 30% with each bounce.
        //The bullet curves slightly towards the player's direction as it moves..


        StartCoroutine(Attack1Coroutine());

    }
    public override void Attack2()
    {
        //Dashes in a straight line (5 units distance) that leaves behind 2 ghostly clones of her for 10 seconds.
        //These clones only deal and take spiritual damage, but only have 5% of the boss max health.
        //They perform Attack 1 when they spawn, 5 seconds after they spawned, and 10 seconds when they spawn.
        //If destroyed before their life time, they do not perform the rest of their Attack 1.
        //Luna is untargetable during the dash and does not take direct damage
        //(Previous debuffs before she dashes continues to affect her).

        //set dashtarget to 5 units infront of its current direction

        Debug.Log("Attack2");
        dashTarget = transform.position + transform.up*5f;
        
        isDashing = true;

        

    }
    public override void Attack3()
    {
        //Fires 7 shots in the air that land randomly on the map after 1 second
        //(with warning area indicators during the second) as AOE circles of diameter 3 that last for 10 seconds
        //(circles land at the same time).
        //Stepping into these circles applies 40% Slow and 40% Sundered to the player and deals 10 basic damage 4 times per seconds.
        //When player leave the circle, debuff from this attack is cleared
        //(The circles should last into other attacks/skills and can overlap with each other.)


        isAttacking = true;
        
    }
    public override void Attack4()
    {
        isAttacking = true;

    }

    public override void Attack5()
    {
        isAttacking = true;
        isAttacking = false;
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


private IEnumerator Attack1Coroutine()
    {
        isAttacking = true;
        for (int i = 0; i < 4; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab4, transform.position, Quaternion.identity);
            BouncyProjectile projectileScript = projectile.GetComponent<BouncyProjectile>();
            projectileScript.SetBoss(this.transform);
            projectileScript.Fire(10f);
            yield return new WaitForSeconds(0.5f);
        }

        isAttacking = false;


    }





}
