using UnityEngine;

public class Hermit_Past : Past_TarotCard
{
    public const float dmgBonusPerUnit = 0.05f;
    public const int spiritVisionBonus = 2;

    public Hermit_Past() : base()
    {
        effects.Add(new HermitPast_Effect());
        
        cardName = "Hermit_Past";
        arcana = Arcana.Hermit;

        GetLocalizedDesc();
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