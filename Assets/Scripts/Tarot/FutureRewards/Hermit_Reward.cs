using UnityEngine;

public class Hermit_Reward : Future_Reward
{
    public const float duration = 10f;
    
    public Hermit_Reward(Future_TarotCard card) : base(Hermit_Future.uses, Hermit_Future.cd, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Hermit skill");
    }

    public override string GetName()
    {
        return "Hermit Skill";
    }
}