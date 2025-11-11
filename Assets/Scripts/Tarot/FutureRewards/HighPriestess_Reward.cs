using UnityEngine;

public class HighPriestess_Reward : Future_Reward
{
    public HighPriestess_Reward(Future_TarotCard card) : base(5, 20f, card)
    {
    }

    public override void TriggerSkill()
    {
        if (IsOnCooldown())
        {
            return;
        }
        // TODO
        Debug.Log("Triggered High Priestess skill");

        DecrementSkillUses();
    }
}