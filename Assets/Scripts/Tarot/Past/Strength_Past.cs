using UnityEngine;

public class Strength_Past : Past_TarotCard
{
    public const float resistanceBonus = 10f;

    public Strength_Past() : base()
    {
        effects.Add(new StrengthPast_Effect());

        cardName = "Strength_Past";
        arcana = Arcana.Strength;

        GetLocalizedDesc();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            Rnd(resistanceBonus)
        };
    }
}