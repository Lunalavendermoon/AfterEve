using UnityEngine;

public class Chariot_Reward : Future_Reward
{
    public Chariot_Reward(Future_TarotCard card) : base(3, 10f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Chariot skill");
    }
}