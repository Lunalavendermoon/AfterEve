using UnityEngine;

public class Chariot_Reward : Future_Reward
{
    const float hasteAmount = 1.5f;
    const float strengthAmount = 1.4f;
    const float duration = 10f;

    public Chariot_Reward(Future_TarotCard card) : base(3, 10f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Chariot skill");
        EffectManager em = PlayerController.instance.gameObject.GetComponent<EffectManager>();
        em.AddBuff(new Haste_Effect(duration, hasteAmount));
        em.AddBuff(new Strength_Effect(duration, strengthAmount));
    }

    public override string GetName()
    {
        return "Chariot Skill";
    }
}