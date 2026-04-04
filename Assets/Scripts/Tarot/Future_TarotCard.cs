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
    }

    /// <summary>
    /// Removes Future-card quest listeners and gives the player a skill reward.
    /// </summary>
    public void CompleteQuest()
    {
        questCompleted = true;
        RewardPlayer();
    }

    protected abstract void RemoveListeners();

    protected virtual void RewardPlayer()
    {
        if (reward != null)
        {
            // TODO give reward to player
        }
        
        RemoveCard(TarotManager.instance);
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        RemoveListeners();
    }

    public string GetQuestText()
    {
        return desc.GetLocalizedString();
    }

    public override string GetDescription()
    {
        return $"{desc.GetLocalizedString()}\n{rewardDesc.GetLocalizedString()}";
    }

    protected void GetLocalizedDesc(int uses, float cd)
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

        reward.SetRewardArguments(rewardDesc, uses, cd);

        SetDescriptionValues();
    }

    protected override void SetTableEntries(string cardName)
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
