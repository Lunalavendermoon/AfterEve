using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Future_TarotCard : TarotCard
{
    public Future_Reward reward = null; // set to public for testing

    public bool questCompleted = false;

    public Future_TarotCard(int q) : base(q)
    {
        GetLocalizedDesc();
    }

    /// <summary>
    /// Removes Future-card quest listeners and gives the player a skill reward.
    /// </summary>
    public void CompleteQuest()
    {
        questCompleted = true;
        RemoveListeners();
        RewardPlayer();
    }

    protected abstract void RemoveListeners();

    protected virtual void RewardPlayer()
    {
        // TODO give reward to player
    }

    public virtual void RemoveCard()
    {
        // TODO remove this card from player's inventory
        if (reward != null)
        {
            // TODO remove skill from player's skill slot
            // this works for 1 skill slot but not multiple...
            PlayerController.instance.futureSkill = null;
        }
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        RemoveCard();
    }

    public abstract string GetQuestText();
}
