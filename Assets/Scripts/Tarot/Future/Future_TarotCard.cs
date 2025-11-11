using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Future_TarotCard : TarotCard
{
    protected Future_Reward reward = null;

    public Future_TarotCard(string s, int q) : base(s, q)
    {
    }

    public abstract void CompleteQuest();

    public virtual void RewardPlayer()
    {
        // TODO give reward to player
    }

    public virtual void RemoveCard()
    {
        // TODO remove this card from player's inventory
        if (reward != null)
        {
            // TODO remove skill from player's skill slot
        }
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        RemoveCard();
    }
}
