using System;
using UnityEngine;

public abstract class Future_Reward
{
    public enum FuturePrefabs
    {
        HighPriestessZone,
        EmpressPulse,
        LoverSummon,
        StrengthZone
    }

    public int usesLeft;
    public float cooldown;
    public float lastUseTime;

    // invoked once all uses of the reward are done
    public static event Action OnRewardFinished;

    bool firstUsage = true;
    
    public Future_Reward(int uses, float cooldown, Future_TarotCard card)
    {
        usesLeft = uses;
        this.cooldown = cooldown;
        if (card != null)
        {
            // card should never be null in actual game - this is just for testing skills in isolation
            OnRewardFinished += card.RemoveCard;
        }
    }

    public void TriggerSkill()
    {
        if (IsOnCooldown())
        {
            Debug.Log($"Skill is on cooldown!");
            return;
        }

        TriggerSkillBehavior();
        DecrementSkillUses();
    }

    protected abstract void TriggerSkillBehavior();

    public bool IsOnCooldown()
    {
        return !firstUsage && cooldown != 0f && (Time.time - lastUseTime < cooldown);
    }

    public void DecrementSkillUses()
    {
        firstUsage = false;

        --usesLeft;
        Debug.Log($"Used skill, {usesLeft} uses left");
        lastUseTime = Time.time;

        if (usesLeft == 0)
        {
            Debug.Log("Skill is completely used!");
            OnRewardFinished?.Invoke();
        }
    }
}