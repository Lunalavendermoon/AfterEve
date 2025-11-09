using UnityEngine;
using System.Collections;
public class KnightofHammersScript : EnemyBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float wanderRadius = 7.5f;
    [SerializeField] private float wanderTime = 3f;

    [SerializeField] int jumpCount = 3;                   // Number of jumps
    [SerializeField] float jumpDistance = 3f;             // Max distance per jump
    [SerializeField] float jumpDuration = 1f;           // Time per jump
    [SerializeField] float hitRadius = 1f;                // Damage radius
    [SerializeField] float jumpHeight = 1.5f;
    void Awake()
    {
        health = enemyAttributes.hitPoints;
        damage = enemyAttributes.damage;
        speed = enemyAttributes.speed;
        visibleRange = enemyAttributes.detection_radius;
        attackRange = enemyAttributes.attackRadius;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed;
        default_enemy_state = new Enemy_Wander(wanderRadius, wanderTime);

        attackCooldown = enemyAttributes.attackRate;

    }

    public override void Attack(Transform target)
    {

        StartCoroutine(JumpAttack(target));
        Debug.Log("Attack Func");
    }





    private IEnumerator JumpAttack(Transform target)
    {
          

        for (int i = 0; i < jumpCount; i++)
        {
            
            Vector3 directionToPlayer = target.position - transform.position;
   
            float distance = directionToPlayer.magnitude;
            Vector3 jumpDirection = directionToPlayer.normalized;

            
            float distanceThisJump = Mathf.Min(distance, jumpDistance);

            // target position
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + jumpDirection * distanceThisJump;

            
            EnableAttack();

            float elapsed = 0f;
            while (elapsed < jumpDuration)
            {
                Debug.Log("Jumping towards: " + targetPos);
                elapsed += Time.deltaTime;
                float t = elapsed / jumpDuration;

                
                float heightOffset = 4 * jumpHeight * t * (1 - t); 
                transform.position = Vector3.Lerp(startPos, targetPos, t) + (-Vector3.forward) * heightOffset;

                yield return null;
            }

            
            transform.position = targetPos;

            // Damage area 
            Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    // Deal damage here
                    Debug.Log("Hit player with landing impact!");
                }
            }

            
            DisableAttack();

            
            yield return new WaitForSeconds(1f);
        }


    }
}
