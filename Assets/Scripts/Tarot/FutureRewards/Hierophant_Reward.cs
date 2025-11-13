using UnityEngine;

public class Hierophant_Reward : Future_Reward
{
    public Hierophant_Reward(Future_TarotCard card) : base(5, 10f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Hierophant skill");
    }
}