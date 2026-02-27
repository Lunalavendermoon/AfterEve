using UnityEngine;

public class Lovers_Past : Past_TarotCard
{
    public const float spiritualDmgBonus = 0.2f;

    public Lovers_Past(int q) : base(q)
    {
        cardName = "Lovers_Past";
        arcana = Arcana.Lovers;
    }

    protected override void ApplyListenersEffects()
    {
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddBuff(new LoversPast_Effect());
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Lovers");

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(spiritualDmgBonus)
        };
    }
}