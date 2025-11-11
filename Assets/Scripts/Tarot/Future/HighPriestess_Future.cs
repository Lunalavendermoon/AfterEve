public class HighPriestess_Future : Future_TarotCard
{
    int spiritualCount = 0;

    int debuffCount = 0;

    // TODO add the actual numbers once design team finalizes :D
    public const int spiritualGoal = 10;
    public const int debuffGoal = 10;

    public HighPriestess_Future(string s, int q) : base(s, q)
    {
        reward = new HighPriestess_Reward(this);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        EnemyBase.OnEnemyDeath += OnEnemyDeathTarot;
    }

    public override void CompleteQuest()
    {
        EnemyBase.OnEnemyDeath -= OnEnemyDeathTarot;
        RewardPlayer();
    }

    private void OnEnemyDeathTarot(DamageInstance dmgInstance, EnemyBase enemy)
    {
        if (dmgInstance.damageType == DamageInstance.DamageType.Spiritual)
        {
            ++spiritualCount;
        }

        if (enemy.GetComponent<EnemyEffectManager>().debuffs.Count != 0)
        {
            ++debuffCount;
        }

        if (spiritualCount >= spiritualGoal || debuffCount >= debuffGoal)
        {
            CompleteQuest();
        }
    }
}