using UnityEngine;

public class Lovers_Reward : Future_Reward
{
    public const float duration = 20f;

    public const float dmgMultiplier = 0.4f;

    public Lovers_Reward(Future_TarotCard card) : base(Lovers_Future.uses, Lovers_Future.cd, card)
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