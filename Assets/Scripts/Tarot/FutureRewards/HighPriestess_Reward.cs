using UnityEngine;
using UnityEngine.Localization;

public class HighPriestess_Reward : Future_Reward
{
    const int zoneRadiusDisplay = 5;
    public const float zoneDuration = 10f;
    public const int cursedAmount = 40;

    public HighPriestess_Reward() : base(HighPriestess_Future.uses, HighPriestess_Future.cd, TarotCard.Arcana.HighPriestess)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.HighPriestessZone, zoneDuration);
    }

    public override void SetRewardArguments(LocalizedString rewardDesc, int displayUses, float displayCooldown)
    {
        rewardDesc.Arguments = new object[] { zoneRadiusDisplay, Mathf.RoundToInt(zoneDuration),
            cursedAmount, Mathf.RoundToInt(displayCooldown), displayUses };
    }
}