using UnityEngine;

public class Strength_Reward : Future_Reward
{
    public Strength_Reward(Future_TarotCard card) : base(3, 5f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Strength skill");
    }
}