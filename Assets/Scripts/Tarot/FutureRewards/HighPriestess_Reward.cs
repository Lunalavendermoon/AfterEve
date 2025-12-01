using UnityEngine;

public class HighPriestess_Reward : Future_Reward
{
    const float zoneDuration = 10f;
    public HighPriestess_Reward(Future_TarotCard card) : base(50, 1f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered High Priestess skill");
        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.HighPriestessZone, zoneDuration);
    }
}