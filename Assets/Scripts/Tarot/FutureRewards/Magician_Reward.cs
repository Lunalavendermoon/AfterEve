using System.Collections;
using UnityEngine;
using UnityEngine.Localization;

public class Magician_Reward : Future_Reward
{
    public const int coinsPerShot = 5;

    public const int damageMultiplier = 2;

    public const float skillDuration = 5f;

    public Magician_Reward() : base(Magician_Future.uses, Magician_Future.cd, TarotCard.Arcana.Magician)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        PlayerController.instance.SetMagicianSkill(true);

        PlayerController.OnCoinsDecrease += OnCoinDecrease;
    }

    void OnCoinDecrease()
    {
        if (PlayerController.instance.GetCoins() < coinsPerShot)
        {
            EndMagicianSkill();
        }
    }

    void EndMagicianSkill()
    {
        if (PlayerController.instance.IsMagicianSkillActive())
        {
            PlayerController.instance.SetMagicianSkill(false);
            PlayerController.OnCoinsDecrease -= OnCoinDecrease;
        }
    }

    public override void SetRewardArguments(LocalizedString rewardDesc, int displayUses, float displayCooldown)
    {
        rewardDesc.Arguments = new object[] { coinsPerShot, Mathf.RoundToInt(skillDuration),
            Mathf.RoundToInt(displayCooldown), displayUses };
    }
}
