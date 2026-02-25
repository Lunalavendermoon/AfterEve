using UnityEngine;

public class HighPriestess_Reward : Future_Reward
{
    public HighPriestess_Reward(Future_TarotCard card) : base(HighPriestess_Future.uses, HighPriestess_Future.cd, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered High Priestess skill");
        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.HighPriestessZone, HighPriestess_Future.zoneDuration);
    }

    public override string GetName()
    {
        return "HighPriestess Skill";
    }
}