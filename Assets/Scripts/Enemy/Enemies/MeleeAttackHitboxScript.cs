using UnityEngine;

public class MeleeAttackHitboxScript : MonoBehaviour
{
    private StandardEnemyBase enemy;
    private bool hasDealtDamage = false;

    void Awake()
    {
        enemy = GetComponentInParent<StandardEnemyBase>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDealtDamage) return;

        if (other.CompareTag("Player"))
        {
            int damage = enemy.enemyAttributes.damage;
            DamageInstance.DamageType dmgType = ToDamageInstanceType(enemy.enemyAttributes.damageType);
            PlayerController.instance.TakeDamage(damage, DamageInstance.DamageSource.Enemy, dmgType);
            hasDealtDamage = true;
            enemy.attackHitConnected = true;
            enemy.DisableAttack();
        }
    }

    private static DamageInstance.DamageType ToDamageInstanceType(EnemyAttributes.DamageType type)
    {
        switch (type)
        {
            case EnemyAttributes.DamageType.Basic: return DamageInstance.DamageType.Physical;
            case EnemyAttributes.DamageType.Spiritual: return DamageInstance.DamageType.Spiritual;
            default: return DamageInstance.DamageType.Mixed;
        }
    }

    private void OnDisable()
    {
        hasDealtDamage = false;
    }
}
