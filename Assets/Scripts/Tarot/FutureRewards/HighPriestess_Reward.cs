using UnityEngine;

public class HighPriestess_Reward : Future_Reward
{
    public HighPriestess_Reward(Future_TarotCard card) : base(5, 20f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered High Priestess skill");
    }
}