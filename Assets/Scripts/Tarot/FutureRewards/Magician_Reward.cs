using UnityEngine;

public class Magician_Reward : Future_Reward
{
    public Magician_Reward(Future_TarotCard card) : base(1, 0f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Magician skill");
    }
}