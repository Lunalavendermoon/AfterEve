using UnityEngine;

public class HighPriestess_Future : Future_TarotCard
{
    int spiritualCount = 0;

    int debuffCount = 0;

    public const int spiritualGoal = 10;
    public const int debuffGoal = 5;

    public const int uses = 5;
    public const float cd = 20f;

    public HighPriestess_Future(int q) : base(q)
    {
        cardName = "HighPriestess_Future";
        reward = new HighPriestess_Reward();
        arcana = Arcana.HighPriestess;
        
        GetLocalizedDesc(uses, cd);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        EnemyBase.OnEnemyDeath += OnEnemyDeathTarot;
    }

    protected override void RemoveListeners()
    {
        EnemyBase.OnEnemyDeath -= OnEnemyDeathTarot;
    }

    private void OnEnemyDeathTarot(DamageInstance dmgInstance, EnemyBase enemy)
    {
        if (dmgInstance.damageType == DamageInstance.DamageType.Spiritual)
        {
            ++spiritualCount;
            RefreshDescription();
        }

        if (enemy.GetComponent<EnemyEffectManager>().debuffs.Count != 0)
        {
            ++debuffCount;
            RefreshDescription();
        }

        if (spiritualCount >= spiritualGoal || debuffCount >= debuffGoal)
        {
            CompleteQuest();
        }
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] { spiritualCount, spiritualGoal, debuffCount, debuffGoal };
    }
}