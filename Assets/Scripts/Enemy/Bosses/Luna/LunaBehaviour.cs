using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemySpawnerScript;

public class LunaBehaviour : BossBehaviourBase
{
    bool isInvulnerable = false;
    int shieldHealth = 200;



    
    [SerializeField] private GameObject projectilePrefab1;
    [SerializeField] private GameObject projectilePrefab3;
    [SerializeField] private GameObject projectilePrefab4;


    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;

    private float shootTimer;


    private void Awake()
    {
        cooldown_time = 4f;
        checkMeleeRange();
        default_enemy_state = new Boss_Attack(this.ChooseAttack());
        
        Debug.Log("Boss Start");

    }

    public override void BossUpdate()
    {
        current_enemy_state.UpdateState(this);
        checkMeleeRange();
    }

    private void checkMeleeRange()
    {
        if(Vector3.Distance(PlayerController.instance.transform.position, transform.position) <= 4f)
        {

            attackProbalities = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 100f };
        }
        else
        {
            attackProbalities = new float[5] { 25.0f, 25.0f, 25.0f, 25.0f, 0f };
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

        isAttacking = true; 
                    
        

        isAttacking = false;


    }
    public override void Attack2()
    {
        //Dashes in a straight line (5 units distance) that leaves behind 2 ghostly clones of her for 10 seconds.
        //These clones only deal and take spiritual damage, but only have 5% of the boss max health.
        //They perform Attack 1 when they spawn, 5 seconds after they spawned, and 10 seconds when they spawn.
        //If destroyed before their life time, they do not perform the rest of their Attack 1.
        //Luna is untargetable during the dash and does not take direct damage
        //(Previous debuffs before she dashes continues to affect her).
        isAttacking = true;


        
        isAttacking = false;

    }
    public override void Attack3()
    {

        isAttacking = true;
        
    }
    public override void Attack4()
    {
        isAttacking = true;

    }

    public override void Attack5()
    {
        isAttacking = true;
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




}
