using System;
using UnityEngine;
using UnityEngine.Localization;

public abstract class Future_Reward
{
    public enum FuturePrefabs
    {
        HighPriestessZone,
        EmpressPulse,
        LoverSummon,
        StrengthZone
    }

    public TarotCard.Arcana arcana;

    public int uses;
    public int usesLeft;
    public float cooldown;
    public float lastUseTime;

    bool firstUsage = true;

    public static event Action OnSkillUsed;

    protected LocalizedString desc;
    
    public Future_Reward(int uses, float cooldown, TarotCard.Arcana arcana)
    {
        this.uses = uses;
        usesLeft = uses;
        this.cooldown = cooldown;
        this.arcana = arcana;

        GetLocalizedDesc();
    }

    public void TriggerSkill()
    {
        if (IsOnCooldown())
        {
            Debug.Log($"Skill is on cooldown!");
            return;
        }

        OnSkillUsed?.Invoke();
        TriggerSkillBehavior();
        DecrementSkillUses();
    }

    protected abstract void TriggerSkillBehavior();

    public bool IsOnCooldown()
    {
        return !firstUsage && cooldown != 0f && (Time.time - lastUseTime < cooldown);
    }

    public void AddUses(int amount)
    {
        usesLeft += amount;
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
            // TODO remove skill from skill slot
        }
    }

    public override string ToString()
    {
        if (IsOnCooldown())
        {
            return $"{arcana}: {usesLeft} uses, {cooldown - Time.time + lastUseTime}s CD";
        }
        return $"{arcana}: {usesLeft} uses, not on CD";
    }

    protected void GetLocalizedDesc()
    {
        desc = new LocalizedString
        {
            TableReference = "FutureRewardTable",
            TableEntryReference = $"{arcana}Reward"
        };

        SetRewardArguments(desc, usesLeft, cooldown);
    }

    public abstract void SetRewardArguments(LocalizedString rewardDesc, int displayUses, float displayCooldown);
}