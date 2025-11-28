using UnityEngine;

public class Empress_Reward : Future_Reward
{
    const float healPercent = 0.5f;

    public Empress_Reward(Future_TarotCard card) : base(5, 5f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Empress skill");
        PlayerController.instance.Heal((int)(PlayerController.instance.playerAttributes.maxHitPoints * healPercent));
        // TODO generate a pulse for 3s that knocks back enemies from the center
    }
}