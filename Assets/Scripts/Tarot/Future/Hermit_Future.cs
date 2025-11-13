using UnityEngine;

public class Hermit_Future : Future_TarotCard
{
    public const int weakPointGoal = 5;
    private int weakPointCount = 0;

    public Hermit_Future(string s, int q) : base(s, q)
    {
        reward = new Hermit_Reward(this);
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
}