using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Hierophant_Reward : Future_Reward
{
    public const int shieldAmount = 5;
    public const float shieldDuration = 10f;

    public Hierophant_Reward(Future_TarotCard card) : base(Hierophant_Future.uses, Hierophant_Future.cd, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Hierophant skill");
        PlayerController.instance.GainHitCountShield(shieldAmount, shieldDuration);
    }

    public override string GetName()
    {
        return "Hierophant Skill";
    }
}