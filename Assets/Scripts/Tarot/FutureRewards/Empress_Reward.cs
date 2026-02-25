using UnityEngine;

public class Empress_Reward : Future_Reward
{
    public const float healPercent = 0.5f;
    public const float pulseDuration = 3f;

    public Empress_Reward(Future_TarotCard card) : base(Empress_Future.uses, Empress_Future.cd, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Empress skill");
        PlayerController.instance.Heal((int)(PlayerController.instance.playerAttributes.maxHitPoints * healPercent));
        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.EmpressPulse, pulseDuration);
    }

    public override string GetName()
    {
        return "Empress Skill";
    }
}