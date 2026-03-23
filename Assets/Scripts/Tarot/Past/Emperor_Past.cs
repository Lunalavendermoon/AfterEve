using UnityEngine;

public class Emperor_Past : Past_TarotCard
{
    PlayerEffectManager effectManager;
    EffectInstance ei = null;

    public const float damageBonus = 0.25f;
    public const int defBonus = 25;

    public Emperor_Past(int q) : base(q)
    {
        cardName = "Emperor_Past";
        arcana = Arcana.Emperor;
    }

    protected override void ApplyListenersEffects()
    {
        effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();

        Player_Idle.OnIdleEnter += OnIdleEnter;
        Player_Idle.OnIdleExit += OnIdleExit;
    }

    void OnIdleEnter()
    {
        ei = effectManager.AddBuff(new EmperorPast_Effect());;
    }

    void OnIdleExit()
    {
        if (ei != null)
        {
            effectManager.RemoveEffect(ei);
        }
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Emperor");

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(damageBonus),
            defBonus
        };
    }
}