using UnityEngine;
using UnityEngine.Localization;

public class Empress_Reward : Future_Reward
{
    public const float healPercent = 0.5f;
    public const float pulseDuration = 3f;

    public Empress_Reward() : base(Empress_Future.uses, Empress_Future.cd, TarotCard.Arcana.Empress)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Empress skill");
        PlayerController.instance.Heal((int)(PlayerController.instance.playerAttributes.maxHitPoints * healPercent));
        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.EmpressPulse, pulseDuration);
    }

    public override void SetRewardArguments(LocalizedString rewardDesc, int displayUses, float displayCooldown)
    {
        rewardDesc.Arguments = new object[] { TarotCard.FormatPercentage(healPercent),
            pulseDuration, Mathf.RoundToInt(displayCooldown), displayUses };
    }
}