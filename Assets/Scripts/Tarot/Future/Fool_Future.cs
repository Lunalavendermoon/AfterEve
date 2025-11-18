using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;

public class Fool_Future : Future_TarotCard
{
    private int cardCount = 0;

    public const int cardGoal = 5;

    public const int foolCoinRewardAmount = 50;

    public Fool_Future(int q) : base(q)
    {
        name = "Fpol_Future";
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        // TODO add card listener
    }

    protected override void RemoveListeners()
    {
        // TODO remove card listener
    }

    private void OnCardObtained()
    {
        ++cardCount;
        if (cardCount >= cardGoal)
        {
            cardCount = 0;
            CompleteQuest();
        }
    }

    protected override void RewardPlayer()
    {
        // TODO give player 50 coins

        // Fool card doesn't use Future_Reward, so we have to manually call RemoveCard() here
        RemoveCard();
    }

    public override string GetQuestText()
    {
        return $"obtain {cardCount}/{cardGoal} cards";
    }
}
