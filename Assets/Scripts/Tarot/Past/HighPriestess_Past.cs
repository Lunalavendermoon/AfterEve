using UnityEngine;

public class HighPriestess_Past : Past_TarotCard
{
    const float debuffDamage = 0.3f;

    public HighPriestess_Past(int q) : base(q)
    {
        cardName = "HighPriestess_Past";
        arcana = Arcana.HighPriestess;
    }

    protected override void ApplyListenersEffects()
    {
        Projectile.OnEnemyHit += HandleEnemyHit;
    }

    private void HandleEnemyHit(EnemyBase enemy)
    {
        if (enemy.enemyEffectManager.debuffs.Count > 0)
        {
            int rawDmg = (int)(debuffDamage * PlayerController.instance.playerAttributes.damage);
            enemy.TakeDamage(rawDmg, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Physical);
        }
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("HighPriestess");

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(debuffDamage)
        };
    }
}