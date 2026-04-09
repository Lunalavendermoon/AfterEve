using UnityEngine;

public class Strength_Future : Future_TarotCard
{
    public const int effectGoal = 3;

    public const int uses = 3;
    public const float cd = 5f;

    public Strength_Future() : base()
    {
        cardName = "Strength_Future";
        reward = new Strength_Reward();
        arcana = Arcana.Strength;
        
        GetLocalizedDesc(uses, cd);
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

    protected override void SetDescriptionValues()
    {
        PlayerEffectManager effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();
        desc.Arguments = new object[] { effectManager.debuffs.Count, effectGoal,
            effectManager.buffs.Count };
    }
}