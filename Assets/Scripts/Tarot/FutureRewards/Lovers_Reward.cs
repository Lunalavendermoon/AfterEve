using UnityEngine;

public class Lovers_Reward : Future_Reward
{
    public Lovers_Reward(Future_TarotCard card) : base(5, 1f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Lovers skill");
    }
}