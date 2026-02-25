using UnityEngine;

public class Hermit_Future : Future_TarotCard
{
    public const int weakPointGoal = 5;
    private int weakPointCount = 0;

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
            if (weakPointCount >= weakPointGoal)
            {
                CompleteQuest();
            }
        }
    }

    public string temp()
    {
        return $"kill {weakPointCount}/{weakPointGoal} enemies by hitting weak point";
    }
}