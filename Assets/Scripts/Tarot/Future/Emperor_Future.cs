public class Emperor_Future : Future_TarotCard
{
    public const int attackRoomGoal = 8;
    public const int damageGoal = 1000;

    private int attackRoomCount = 0;
    private int damageCount = 0;

    public Emperor_Future(int q) : base(q)
    {
        name = "Emperor_Future";
        reward = new Emperor_Reward(this);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        // TODO room listener
        PlayerController.OnDamageTaken += OnAttackReceived;
    }

    protected override void RemoveListeners()
    {
        // TODO room listener
        PlayerController.OnDamageTaken -= OnAttackReceived;
    }

    private void OnAttackReceived(DamageInstance dmg)
    {
        if (dmg.damageSource != DamageInstance.DamageSource.Enemy)
        {
            return;
        }

        damageCount += dmg.beforeReduction;
        ++attackRoomCount;

        if (damageCount >= damageGoal || attackRoomCount >= attackRoomGoal)
        {
            CompleteQuest();
        }
    }

    private void OnRoomChanged()
    {
        attackRoomCount = 0;
    }
}