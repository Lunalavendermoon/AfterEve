using UnityEngine;

public class Lovers_Future : Future_TarotCard
{
    public const int spendGoal = 40;
    private int spendCount = 0;

    public const int uses = 5;
    public const float cd = 1f;

    public Lovers_Future(int q) : base(q)
    {
        cardName = "Lovers_Future";
        reward = new Lovers_Reward(this);
        arcana = Arcana.Lovers;
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
        RefreshDescription();
        if (spendCount >= spendGoal)
        {
            CompleteQuest();
        }
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Lovers");
        
        rewardDesc.Arguments = new object[] { Mathf.RoundToInt(Lovers_Reward.duration),
            FormatPercentage(Lovers_Reward.dmgMultiplier), Mathf.RoundToInt(cd), uses };

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] { spendCount, spendGoal };
    }
}