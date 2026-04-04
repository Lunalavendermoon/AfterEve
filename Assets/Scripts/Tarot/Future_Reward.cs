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
    int skillIndex = 0;

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
        lastUseTime = Time.time;

        if (usesLeft == 0)
        {
            PlayerController.instance.futureSkills[skillIndex] = null;
        }
    }

    public override string ToString()
    {
        if (IsOnCooldown())
        {
            return $"{arcana}: {usesLeft} uses, {(int)(cooldown - Time.time + lastUseTime)}s CD";
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

    public void SetSkillIndex(int newIdx)
    {
        skillIndex = newIdx;
    }

    public abstract void SetRewardArguments(LocalizedString rewardDesc, int displayUses, float displayCooldown);
}