using UnityEngine;

public class Emperor_Past : Past_TarotCard
{
    PlayerEffectManager effectManager;
    EffectInstance ei = null;

    public const float damageBonus = 0.25f;
    public const int defBonus = 25;

    public Emperor_Past() : base()
    {
        cardName = "Emperor_Past";
        arcana = Arcana.Emperor;

        GetLocalizedDesc();
    }

    protected override void ApplyListenersEffects(bool muted = false)
    {
        effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();

        Player_Idle.OnIdleEnter += OnIdleEnter;
        Player_Idle.OnIdleExit += OnIdleExit;
    }

    protected override void RemoveListeners(bool muted = false)
    {
        Player_Idle.OnIdleEnter -= OnIdleEnter;
        Player_Idle.OnIdleExit -= OnIdleExit;
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

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(damageBonus),
            defBonus
        };
    }
}