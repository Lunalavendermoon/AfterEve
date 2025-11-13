using UnityEngine;

public class Empress_Reward : Future_Reward
{
    public Empress_Reward(Future_TarotCard card) : base(5, 5f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Empress skill");
    }
}