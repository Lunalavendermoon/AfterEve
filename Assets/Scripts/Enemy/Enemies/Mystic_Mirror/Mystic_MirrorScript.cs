using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mystic_MirrorScript : EnemyBase
{
    void Awake()
    {
        if (baseEnemyAttributes != null)
        {
            enemyAttributes = Instantiate(baseEnemyAttributes);
            health = Mathf.Max(1, enemyAttributes.maxHitPoints);
            speed = enemyAttributes.speed;
        }
        else
            health = Mathf.Max(1, health);
        default_enemy_state = new Enemy_Wander();

    }



    public override void Attack(Transform target)
    {

        StartCoroutine(TeleportAttack(target));
    }

    public override void EnemyUpdate()
    {
        base.EnemyUpdate();
        Debug.Log(current_enemy_state);
        if (agent != null)
            agent.endReachedDistance = 5f;
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

        if (player == null) return;
        const float teleportRadius = 5f;
        const int attempts = 20;
        Vector3 chosen = transform.position;
        bool found = false;
        for (int i = 0; i < attempts; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * teleportRadius;
            Vector2 candidate2D = (Vector2)player.position + randomOffset;
            bool inside = false;
            // Narrative room
            if (NarrativeRoomManager.instance != null &&
                NarrativeRoomManager.instance.IsPointInsideCurrentRoom(candidate2D, 0f))
            {
                inside = true;
            }
            else if (GameManager.instance != null &&
                     GameManager.instance.IsWorldPointInsideMap(candidate2D, 0f))
            {
                inside = true;
            }
            if (inside)
            {
                chosen = new Vector3(candidate2D.x, candidate2D.y, transform.position.z);
                found = true;
                break;
            }
        }
        if (!found) return; // fallback: don't teleport if no valid point found
        transform.position = chosen;
        if (enemyAttributes == null) return;
        if (Vector3.Distance(transform.position, player.position) <= enemyAttributes.attackRadius)
        {
            var effectManager = PlayerController.instance?.gameObject?.GetComponent<PlayerEffectManager>();
            if (effectManager != null && PlayerController.instance?.playerAttributes != null)
                effectManager.AddEffect(new Confused_Effect(5f), PlayerController.instance.playerAttributes);
        }

    }



}
