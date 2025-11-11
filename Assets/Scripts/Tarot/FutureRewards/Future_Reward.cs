using System;
using UnityEngine;

public abstract class Future_Reward
{
    public int usesLeft;
    public float cooldown;
    public float lastUseTime;

    // invoked once all uses of the reward are done
    public static event Action OnRewardFinished;
    
    public Future_Reward(int uses, float cooldown, Future_TarotCard card)
    {
        usesLeft = uses;
        this.cooldown = cooldown;
        OnRewardFinished += card.RemoveCard;
        lastUseTime = Time.time - 1f;
    }

    public abstract void TriggerSkill();

    public bool IsOnCooldown()
    {
        return cooldown != 0f && (Time.time - lastUseTime < cooldown);
    }

    public void DecrementSkillUses()
    {
        --usesLeft;
        lastUseTime = Time.time;

        if (usesLeft == 0)
        {
            SignalRewardFinished();
        }
    }

    public void SignalRewardFinished()
    {
        OnRewardFinished.Invoke();
    }
}