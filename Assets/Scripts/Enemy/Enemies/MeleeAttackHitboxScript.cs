using UnityEngine;

public class MeleeAttackHitboxScript : MonoBehaviour
{
    private EnemyBase enemy;
    private bool hasDealtDamage = false;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDealtDamage ) return;


        if (other.CompareTag("Player"))
        {

            //PlayerController.instance.playerAttributes.TakeDamage(enemy.damage);
            hasDealtDamage = true;
            Debug.Log("Player hit for " + enemy.enemyAttributes.damage);
            enemy.DisableAttack();
        }
    }


    private void OnDisable()
    {
        hasDealtDamage = false; 
    }
}
