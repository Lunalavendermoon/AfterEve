using System.Collections;
using UnityEngine;

public class LunaGhostBehaviour : EnemyBase
{

    [SerializeField] private GameObject projectilePrefab;


    private void Awake()
    {

        health = enemyAttributes.maxHitPoints;

        speed = enemyAttributes.speed;

        default_enemy_state = new Enemy_Attack();

        attack_timer = 0.0f;

        isAttacking = false;
    }


    public override void Attack(Transform target)
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        Debug.Log("Entered Coroutine");
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 4; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            BouncyProjectile projectileScript = projectile.GetComponent<BouncyProjectile>();
            projectileScript.SetBoss(this.transform);
            projectileScript.Fire(10f);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(4f);

        for (int i = 0; i < 4; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            BouncyProjectile projectileScript = projectile.GetComponent<BouncyProjectile>();
            projectileScript.SetBoss(this.transform);
            projectileScript.Fire(10f);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < 4; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            BouncyProjectile projectileScript = projectile.GetComponent<BouncyProjectile>();
            projectileScript.SetBoss(this.transform);
            projectileScript.Fire(10f);
            yield return new WaitForSeconds(0.5f);
        }

        isAttacking = false;

        Destroy(gameObject);


    }
}
