using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Hierophant_Reward : Future_Reward
{
    const int shieldAmount = 5;
    const float shieldDuration = 10f;

    public Hierophant_Reward(Future_TarotCard card) : base(5, 10f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Hierophant skill");
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddEffect(
            new HitCountShield_Effect(shieldAmount, shieldDuration)
        );
    }
}