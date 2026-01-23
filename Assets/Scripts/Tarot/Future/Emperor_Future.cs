public class Emperor_Future : Future_TarotCard
{
    public const int attackRoomGoal = 8;
    public const int damageGoal = 1000;

    private int attackRoomCount = 0;
    private int damageCount = 0;

    public Emperor_Future(int q) : base(q)
    {
        cardName = "Emperor_Future";
        reward = new Emperor_Reward(this);
        arcana = Arcana.Emperor;
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        GameManager.OnRoomChange += OnRoomChanged;
        PlayerController.OnDamageTaken += OnAttackReceived;
    }

    protected override void RemoveListeners()
    {
        GameManager.OnRoomChange -= OnRoomChanged;
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

    public override string GetQuestText()
    {
        return $"receive {attackRoomCount}/{attackRoomGoal} in one room OR take {damageCount}/{damageGoal} enemy dmg";
    }
}