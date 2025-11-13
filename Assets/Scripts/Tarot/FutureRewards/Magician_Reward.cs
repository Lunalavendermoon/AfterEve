using UnityEngine;

public class Magician_Reward : Future_Reward
{
    public Magician_Reward(Future_TarotCard card) : base(3, 10f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Magician skill");
    }
}
