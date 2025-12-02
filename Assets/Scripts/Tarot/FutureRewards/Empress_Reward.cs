using UnityEngine;

public class Empress_Reward : Future_Reward
{
    const float healPercent = 0.5f;
    const float pulseDuration = 3f;

    public Empress_Reward(Future_TarotCard card) : base(5, 5f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Empress skill");
        PlayerController.instance.Heal((int)(PlayerController.instance.playerAttributes.maxHitPoints * healPercent));
        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.EmpressPulse, pulseDuration);
    }
}