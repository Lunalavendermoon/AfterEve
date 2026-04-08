using UnityEngine;

public class Fool_Present : Present_TarotCard
{
    float[] fireRateIncrease = {1.2f, 1.3f, 1.4f, 1.5f, 1.6f};
    float[] additionalDmg = {.2f, .25f, .3f, .35f, .4f};

    EnemyBase lastEnemy;

    public Fool_Present(int q) : base(q)
    {
        cardName = "Fool_Present";
        arcana = Arcana.Fool;
        
        lastEnemy = null;

        AddNewLevelEffects();

        GetLocalizedDesc();
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
        if (lastEnemy != enemy)
        {
            // Deal additional damage to enemy
            enemy.TakeDamage((int)(additionalDmg[level] * PlayerController.instance.playerAttributes.damage),
                DamageInstance.DamageSource.Player, DamageInstance.DamageType.Physical);
        }
        lastEnemy = enemy;
    }

    protected override void AddNewLevelEffects()
    {
        effects.Add(new FireRate_Effect(-1, fireRateIncrease[level]));
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] {
            FormatPlusOnePercentage(fireRateIncrease[level]),
            FormatPercentage(additionalDmg[level])
        };
    }
}
