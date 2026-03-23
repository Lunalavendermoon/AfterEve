using UnityEngine;

public class Strength_Future : Future_TarotCard
{
    public const int effectGoal = 3;

    public const int uses = 3;
    public const float cd = 5f;

    public Strength_Future(int q) : base(q)
    {
        cardName = "Strength_Future";
        reward = new Strength_Reward(this);
        arcana = Arcana.Strength;
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        PlayerEffectManager.OnEffectAdded += OnEffectApplied;
    }

    protected override void RemoveListeners()
    {
        PlayerEffectManager.OnEffectAdded -= OnEffectApplied;
    }

    private void OnEffectApplied()
    {
        PlayerEffectManager effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();
        RefreshDescription();
        if (effectManager.debuffs.Count >= effectGoal || effectManager.buffs.Count >= effectGoal)
        {
            CompleteQuest();
        }
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Strength");
        
        // NOTE - hardcoded strings for damage interval and zone size
        rewardDesc.Arguments = new object[] { Strength_Zone.damage, "0.5", "2",
            FormatPercentage(Strength_Zone.enemySlowAmount), Strength_Reward.maxZoneDistance,
            Mathf.RoundToInt(Strength_Reward.zoneDuration),
            Mathf.RoundToInt(cd), uses };

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        PlayerEffectManager effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();
        desc.Arguments = new object[] { effectManager.debuffs.Count, effectGoal,
            effectManager.buffs.Count };
    }
}