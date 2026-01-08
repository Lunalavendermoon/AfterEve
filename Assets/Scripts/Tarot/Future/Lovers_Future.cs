using UnityEngine;

public class Lovers_Future : Future_TarotCard
{
    public const int spendGoal = 40;
    private int spendCount = 0;

    public Lovers_Future(int q) : base(q)
    {
        name = "Lovers_Future";
        reward = new Lovers_Reward(this);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        PlayerController.OnCoinsSpentAtShop += OnGoldSpentAtMerchant;
    }

    protected override void RemoveListeners()
    {
        PlayerController.OnCoinsSpentAtShop -= OnGoldSpentAtMerchant;
    }

    private void OnGoldSpentAtMerchant(int amount)
    {
        spendCount += amount;
        if (spendCount >= spendGoal)
        {
            CompleteQuest();
        }
    }

    public override string GetQuestText()
    {
        return $"spend {spendCount}/{spendGoal} or more gold at a merchant";
    }
}