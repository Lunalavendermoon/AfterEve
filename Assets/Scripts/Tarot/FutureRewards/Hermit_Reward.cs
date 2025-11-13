using UnityEngine;

public class Hermit_Reward : Future_Reward
{
    public Hermit_Reward(Future_TarotCard card) : base(7, 10f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Hermit skill");
    }
}