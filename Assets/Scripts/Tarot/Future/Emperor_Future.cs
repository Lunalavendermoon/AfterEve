using UnityEngine;

public class Emperor_Future : Future_TarotCard
{
    public const int attackRoomGoal = 8;
    public const int damageGoal = 1000;

    private int attackRoomCount = 0;
    private int damageCount = 0;

    public const int uses = 5;
    public const float cd = 5f;

    public Emperor_Future(int q) : base(q)
    {
        cardName = "Emperor_Future";
        reward = new Emperor_Reward();
        arcana = Arcana.Emperor;
        
        GetLocalizedDesc(uses, cd);
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
        RefreshDescription();

        if (damageCount >= damageGoal || attackRoomCount >= attackRoomGoal)
        {
            CompleteQuest();
        }
    }

    private void OnRoomChanged()
    {
        attackRoomCount = 0;
        RefreshDescription();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] { attackRoomCount, attackRoomGoal, damageCount, damageGoal };
    }
}