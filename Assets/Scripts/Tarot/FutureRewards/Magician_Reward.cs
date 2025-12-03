using System.Collections;
using UnityEngine;

public class Magician_Reward : Future_Reward
{
    public static int coinsPerShot = 5;

    public static int damageMultiplier = 2;

    public static float skillDuration = 5f;

    public Magician_Reward(Future_TarotCard card) : base(3, 10f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO start a 5-second timer and call EndMagicianSkill() at the end
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
}
