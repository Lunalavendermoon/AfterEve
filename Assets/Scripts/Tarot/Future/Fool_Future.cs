using UnityEngine;
using UnityEngine.Localization;

public class Fool_Future : Future_TarotCard
{
    private int cardCount = 0;

    public const int cardGoal = 5;

    public const int foolCoinRewardAmount = 50;

    public Fool_Future() : base()
    {
        cardName = "Fool_Future";
        arcana = Arcana.Fool;
        
        GetLocalizedDesc();
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        TarotManager.OnObtainCard += OnCardObtained;
    }

    protected override void RemoveListeners()
    {
        TarotManager.OnObtainCard -= OnCardObtained;
    }

    private void OnCardObtained(Arcana _)
    {
        ++cardCount;
        RefreshDescription();
        if (cardCount >= cardGoal)
        {
            cardCount = 0;
            CompleteQuest();
        }
    }

    protected override void RewardPlayer()
    {
        PlayerController.instance.ChangeCoins(foolCoinRewardAmount);

        RemoveCard(TarotManager.instance);
        TarotManager.instance.RemoveCard(this);
    }

    protected override void GetLocalizedDesc()
    {
        desc = new LocalizedString
        {
            TableReference = "FutureTarotTable"
        };
        rewardDesc = new LocalizedString
        {
            TableReference = "FutureRewardTable"
        };

        SetTableEntries(arcana.ToString());
        
        rewardDesc.Arguments = new object[] { foolCoinRewardAmount };

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] { cardCount, cardGoal };
    }
}
