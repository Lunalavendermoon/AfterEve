using UnityEngine;

public class HighPriestess_Present : Present_TarotCard
{
    float[] applyProbability = {.25f, .3f, .4f, .5f, .6f};
    float[] spiritualApplyProbability = {.5f, .6f, .7f, .8f, 1};
    float weakPercent = .2f;

    int normalDuration = 3;
    int spiritualDuration = 5;

    Weak_Effect weakDebuff;
    
    PlayerGun playerGun;

    public HighPriestess_Present(int q) : base(q)
    {
        cardName = "HighPriestess_Present";
        arcana = Arcana.HighPriestess;

        weakDebuff = new Weak_Effect(3, weakPercent);
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
        bool applyWeak;
        if (PlayerController.instance.IsInSpiritualVision())
        {
            applyWeak = random < spiritualApplyProbability[level];
        }
        else
        {
            applyWeak = random < applyProbability[level];
        }
        if (applyWeak)
        {
            enemy.enemyEffectManager.AddEffect(weakDebuff, enemy.enemyAttributes);
        }
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] {
            FormatPercentage(applyProbability[level]),
            normalDuration,
            FormatPercentage(weakPercent),
            FormatPercentage(spiritualApplyProbability[level]),
            spiritualDuration
        };
    }
}
