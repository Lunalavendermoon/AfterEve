using UnityEngine;

public class Lovers_Reward : Future_Reward
{
    const float duration = 20f;

    public Lovers_Reward(Future_TarotCard card) : base(5, 1f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Lovers skill");
        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.LoverSummon, duration);
    }

    public override string GetName()
    {
        return "Lovers Skill";
    }
}