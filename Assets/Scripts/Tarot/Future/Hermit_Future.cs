using UnityEngine;

public class Hermit_Future : Future_TarotCard
{
    public const int weakPointGoal = 5;
    private int weakPointCount = 0;

    public const int uses = 7;
    public const float cd = 10f;

    public Hermit_Future(int q) : base(q)
    {
        cardName = "Hermit_Future";
        reward = new Hermit_Reward(this);
        arcana = Arcana.Hermit;
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        EnemyBase.OnEnemyDeath += OnEnemyDefeated;
    }

    protected override void RemoveListeners()
    {
        EnemyBase.OnEnemyDeath -= OnEnemyDefeated;
    }

    private void OnEnemyDefeated(DamageInstance dmg, EnemyBase _)
    {
        if (dmg.hitWeakPoint)
        {
            ++weakPointCount;
            RefreshDescription();
            if (weakPointCount >= weakPointGoal)
            {
                CompleteQuest();
            }
        }
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Hermit");
        
        rewardDesc.Arguments = new object[] { Mathf.RoundToInt(Hermit_Reward.duration),
            Mathf.RoundToInt(cd), uses };

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] { weakPointGoal, weakPointCount };
    }
}