using UnityEngine;

public class Hermit_Past : Past_TarotCard
{
    public const float dmgBonusPerUnit = 0.05f;
    public const int spiritVisionBonus = 2;

    public Hermit_Past(int q) : base(q)
    {
        cardName = "Hermit_Past";
        arcana = Arcana.Hermit;
    }

    protected override void ApplyListenersEffects()
    {
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddBuff(new HermitPast_Effect());
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Hermit");

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(dmgBonusPerUnit),
            spiritVisionBonus
        };
    }
}