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
        em.AddEffect(new Haste_Effect(duration, hasteAmount));
        em.AddEffect(new Strength_Effect(duration, strengthAmount));
    }
}