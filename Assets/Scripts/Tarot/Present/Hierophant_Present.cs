using UnityEngine;

public class Hierophant_Present : Present_TarotCard
{
    int[] enemiesChained = { 3, 4, 5, 6, 7 };
    int[] distance = { 5, 6, 7, 8, 9 };
    float[] chainTime = { 5f, 5f, 5f, 5f, 5f };
    float[] chainDmg = { .3f, .35f, .4f, .45f, .5f };
    float[] shieldIncrease = { .01f, .02f, .03f, .04f, .05f };

    float chainRadius = 5f;

    float timeBetweenTrigger = 1f;
    float timer;

    public Hierophant_Present(int q) : base(q)
    {
        cardName = "Hierophant_Present";
        arcana = Arcana.Hierophant;

        timer = 0f;

        GetLocalizedDesc();
    }

    protected override void ApplyListeners()
    {
        Projectile.OnEnemyHitWithDamage += HandleEnemyHit;
    }

    protected override void RemoveListeners()
    {
        Projectile.OnEnemyHitWithDamage -= HandleEnemyHit;
    }

    void HandleEnemyHit(EnemyBase enemy, int physicalDamage, int spiritualDamage)
    {
        int cumulativeShield = 0;
        for (int i = 0; i < enemiesChained[level]; i++)
        {
            GameObject temp = FindNearestUnchainedEnemy(enemy.gameObject);
            if (temp != null)
            {
                temp.GetComponent<EnemyBase>().Chain(chainTime[level]);
                if (spiritualDamage > 0)
                {
                    cumulativeShield += (int)(PlayerController.instance.playerAttributes.maxHitPoints * shieldIncrease[level] * 2);
                }
                else
                {
                    cumulativeShield += (int)(PlayerController.instance.playerAttributes.maxHitPoints * shieldIncrease[level]);
                }
            }
        }
        DamageAllChainedEnemies(physicalDamage, spiritualDamage);

        if (timer <= 0f && cumulativeShield > 0)
        {
            PlayerController.instance.GainRegularShield(cumulativeShield);
            timer = timeBetweenTrigger;
        }
    }

    public override void UpdateCard()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
    }

    GameObject FindNearestUnchainedEnemy(GameObject justHitEnemy)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(justHitEnemy.transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= chainRadius &&
                !enemy.GetComponent<EnemyBase>().IsChained() && enemy != justHitEnemy)
            {
                closestDistance = distance;
                nearest = enemy;
            }
        }
        return nearest;
    }

    void DamageAllChainedEnemies(int physicalDamage, int spiritualDamage)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyBase>().IsChained())
            {
                if (physicalDamage > 0)
                {
                    enemy.GetComponent<EnemyBase>().TakeDamage((int)(physicalDamage * chainDmg[level]), DamageInstance.DamageSource.Player, DamageInstance.DamageType.Physical);
                }
                if (spiritualDamage > 0)
                {
                    enemy.GetComponent<EnemyBase>().TakeDamage((int)(spiritualDamage * chainDmg[level]), DamageInstance.DamageSource.Player, DamageInstance.DamageType.Spiritual);
                }
            }
        }
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            enemiesChained[level],
            distance[level],
            Rnd(chainTime[level]),
            FormatPercentage(chainDmg[level]),
            FormatPercentage(shieldIncrease[level]),
            Rnd(timeBetweenTrigger)
        };
    }
}
