using UnityEngine;

public class Empress_Present : Present_TarotCard
{
    float[] healProbability = {.2f, .25f, .3f, .25f, .4f};
    float[] healPercent = {.5f, .7f, .9f, .11f, .13f};

    public Empress_Present(int q) : base(q)
    {
        name = "Empress_Present";
    }

    void OnEnable()
    {
        Projectile.OnEnemyHit += HandleEnemyHit;
    }

    void OnDisable()
    {
        Projectile.OnEnemyHit -= HandleEnemyHit;
    }

    private void HandleEnemyHit(EnemyBase enemy)
    {
        int random = Random.Range(0,100);
        if(random < healProbability[level] * 100)
        {
            // round down
            int healAmount = (int)(PlayerController.instance.playerAttributes.maxHitPoints * healPercent[level]);
            int newHealth = PlayerController.instance.GetHealth() + healAmount;
                PlayerController.instance.Heal(healAmount);
            if(newHealth > PlayerController.instance.playerAttributes.maxHitPoints)
            {
                enemy.TakeDamage(healAmount, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Basic);
            }
        }
    }
}
