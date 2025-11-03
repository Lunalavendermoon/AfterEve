using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mystic_MirrorScript : EnemyBase
{
    void Awake()
    {
        health = enemyAttributes.hitPoints;
        damage = enemyAttributes.damage;
        speed = enemyAttributes.speed;
        visibleRange = enemyAttributes.detection_radius;
        attackRange = enemyAttributes.attackRadius;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed;
        default_enemy_state = new Enemy_Chase();
        agent.stoppingDistance = 5f;
        attackCooldown = enemyAttributes.attackRate;

    }


    public override void Attack(Transform target)
    {

        StartCoroutine(TeleportAttack(target));
    }



    public override void Die()
    {
        //TODO
    }
    private IEnumerator TeleportAttack(Transform target)
    {
        float totalDuration = 5f;
        float teleportInterval = 5f;
        float elapsed = 0f;
        isAttacking = true;

        while (elapsed < totalDuration)
        {

            yield return new WaitForSeconds(teleportInterval);
            elapsed += teleportInterval;
            TeleportNearPlayer(target);
            isAttacking = false;
        }


    }

    private void TeleportNearPlayer(Transform player)
    {
        // Similar to wander code
        Vector2 randomOffset = Random.insideUnitCircle * 5f;
        Vector3 newPos = new Vector3(
            player.position.x + randomOffset.x,
            player.position.y + randomOffset.y,
            transform.position.z 
        );

        transform.position = newPos;
        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {

            Debug.Log("Applying Confusion Effect to Player");
        }

    }



}
