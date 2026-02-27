using UnityEngine;

public class Empress_Present : Present_TarotCard
{
    float[] healProbability = {.2f, .25f, .3f, .25f, .4f};
    float[] healPercent = {.5f, .7f, .9f, .11f, .13f};

    float timeBetweenTrigger = 1f;

    float timer;

    public Empress_Present(int q) : base(q)
    {
        cardName = "Empress_Present";
        arcana = Arcana.Empress;

        timer = timeBetweenTrigger + 1f;
    }

    protected override void ApplyListeners()
    {
        Projectile.OnEnemyHit += HandleEnemyHit;
    }

    protected override void RemoveListeners()
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
            if(newHealth > PlayerController.instance.playerAttributes.maxHitPoints && timer <= 0f)
            {
                enemy.TakeDamage(healAmount, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Physical);
                timer = timeBetweenTrigger;
            }
        }
    }

    public override void UpdateCard()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();

        SetTableEntries("Empress");
        
        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] {
            FormatPercentage(healProbability[level]),
            FormatPercentage(healPercent[level]),
            Rnd(timeBetweenTrigger)
        };
    }
}
