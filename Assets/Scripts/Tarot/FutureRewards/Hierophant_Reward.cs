using UnityEngine;
using UnityEngine.Localization;

public class Hierophant_Reward : Future_Reward
{
    public const int shieldAmount = 5;
    public const float shieldDuration = 10f;

    public Hierophant_Reward() : base(Hierophant_Future.uses, Hierophant_Future.cd, TarotCard.Arcana.Hierophant)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        PlayerController.instance.GainHitCountShield(shieldAmount, shieldDuration);
    }

    public override void SetRewardArguments(LocalizedString rewardDesc, int displayUses, float displayCooldown)
    {
        rewardDesc.Arguments = new object[] { shieldAmount,
            Mathf.RoundToInt(shieldDuration), Mathf.RoundToInt(displayCooldown), displayUses };
    }
}