using UnityEngine;

public class HighPriestess_Past : Past_TarotCard
{
    const float debuffDamage = 0.3f;

    public HighPriestess_Past() : base()
    {
        cardName = "HighPriestess_Past";
        arcana = Arcana.HighPriestess;

        GetLocalizedDesc();
    }

    protected override void ApplyListenersEffects(bool muted = false)
    {
        Projectile.OnEnemyHit += HandleEnemyHit;
    }

    protected override void RemoveListeners(bool muted = false)
    {
        Projectile.OnEnemyHit -= HandleEnemyHit;
    }

    private void HandleEnemyHit(EnemyBase enemy)
    {
        if (enemy.enemyEffectManager.debuffs.Count > 0)
        {
            int rawDmg = (int)(debuffDamage * PlayerController.instance.playerAttributes.damage);
            enemy.TakeDamage(rawDmg, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Physical);
        }
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(debuffDamage)
        };
    }
}