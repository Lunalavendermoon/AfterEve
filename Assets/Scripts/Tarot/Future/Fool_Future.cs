using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;

public class Fool_Future : Future_TarotCard
{
    private int cardCount = 0;

    public const int cardsNeededForReward = 5;

    public const int foolCoinRewardAmount = 50;

    public Fool_Future(string s, int q) : base(s, q)
    {
    }

    void OnEnable()
    {
        // TODO add card listener
    }

    void OnDisable()
    {
        // TODO remove card listener
    }

    private void OnCardObtained()
    {
        ++cardCount;
        if (cardCount >= cardsNeededForReward)
        {
            cardCount = 0;
            RewardPlayer();
        }
    }

    public override void RewardPlayer()
    {
        // TODO give player 50 coins
    }
}
