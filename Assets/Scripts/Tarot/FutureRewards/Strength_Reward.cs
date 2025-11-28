using UnityEngine;

public class Strength_Reward : Future_Reward
{
    const float zoneDuration = 5f;

    public Strength_Reward(Future_TarotCard card) : base(3, 5f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        // TODO
        Debug.Log("Triggered Strength skill");
        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.StrengthZone, zoneDuration);
    }
}