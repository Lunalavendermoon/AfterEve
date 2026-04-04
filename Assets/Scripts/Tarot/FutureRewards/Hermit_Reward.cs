using UnityEngine;
using UnityEngine.Localization;

public class Hermit_Reward : Future_Reward
{
    public const float duration = 10f;
    
    public Hermit_Reward() : base(Hermit_Future.uses, Hermit_Future.cd, TarotCard.Arcana.Hermit)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Hermit skill");
    }

    public override void SetRewardArguments(LocalizedString rewardDesc, int displayUses, float displayCooldown)
    {
        rewardDesc.Arguments = new object[] { Mathf.RoundToInt(duration),
            Mathf.RoundToInt(displayCooldown), displayUses };
    }
}