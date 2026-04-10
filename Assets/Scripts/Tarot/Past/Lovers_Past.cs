using UnityEngine;

public class Lovers_Past : Past_TarotCard
{
    public const float spiritualDmgBonus = 0.2f;

    public Lovers_Past() : base()
    {
        effects.Add(new LoversPast_Effect());

        cardName = "Lovers_Past";
        arcana = Arcana.Lovers;

        GetLocalizedDesc();
    }

    protected override void ApplyListenersEffects(bool muted = false)
    {
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddBuff(new LoversPast_Effect(), muted);
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(spiritualDmgBonus)
        };
    }
}