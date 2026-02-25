using System.Collections;
using UnityEngine;

public class Magician_Reward : Future_Reward
{
    public const int coinsPerShot = 5;

    public const int damageMultiplier = 2;

    public const float skillDuration = 5f;

    public Magician_Reward(Future_TarotCard card) : base(Magician_Future.uses, Magician_Future.cd, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Magician skill");

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

    public override string GetName()
    {
        return "Magician Skill";
    }
}
