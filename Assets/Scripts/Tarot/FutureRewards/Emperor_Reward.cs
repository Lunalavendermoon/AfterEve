using UnityEngine;

public class Emperor_Reward : Future_Reward
{
    public Emperor_Reward(Future_TarotCard card) : base(7, 10f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Emperor skill");
    }
}