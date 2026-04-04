using UnityEngine;
using UnityEngine.Localization;

public class Lovers_Reward : Future_Reward
{
    public const float duration = 20f;

    public const float dmgMultiplier = 0.4f;

    public Lovers_Reward() : base(Lovers_Future.uses, Lovers_Future.cd, TarotCard.Arcana.Lovers)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.LoverSummon, duration);
    }

    public override void SetRewardArguments(LocalizedString rewardDesc, int displayUses, float displayCooldown)
    {
        rewardDesc.Arguments = new object[] { Mathf.RoundToInt(duration),
            TarotCard.FormatPercentage(dmgMultiplier), Mathf.RoundToInt(displayCooldown), displayUses };
    }
}