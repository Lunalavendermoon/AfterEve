using UnityEngine;

public class Magician_Reward : Future_Reward
{
    public Magician_Reward(Future_TarotCard card) : base(1, 0f, card)
    {
    }

    public override void TriggerSkill()
    {
        // don't need to check for cooldown because the skill has only 1 use

        // TODO
        Debug.Log("Triggered Magician skill");

        DecrementSkillUses();
    }
}