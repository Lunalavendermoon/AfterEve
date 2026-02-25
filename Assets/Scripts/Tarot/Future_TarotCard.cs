using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;

public abstract class Future_TarotCard : TarotCard
{
    public Future_Reward reward = null; // set to public for testing

    public bool questCompleted = false;

    protected LocalizedString rewardDesc;

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

    public string GetQuestText()
    {
        return desc.GetLocalizedString();
    }

    public override string GetDescription()
    {
        return $"{desc.GetLocalizedString()}\n{rewardDesc.GetLocalizedString()}";
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
    }

    protected void SetTableEntries(string cardName)
    {
        desc.TableEntryReference = $"{cardName}Future";

        rewardDesc.TableEntryReference = $"{cardName}Reward";
    }

    protected virtual void SetDescriptionValues()
    {
        // override in each child
    }

    protected void RefreshDescription()
    {
        SetDescriptionValues();
        desc.RefreshString();
    }
}
