using UnityEngine;

public class Empress_Past : Past_TarotCard
{
    const float currentHpBonus = 0.05f;
    const float maxHpBonus = 0.03f;

    public Empress_Past() : base()
    {
        effects.Add(new EmpressPast_Effect());

        cardName = "Empress_Past";
        arcana = Arcana.Empress;

        GetLocalizedDesc();
    }

    public static int GetDamageBonus()
    {
        return (int)(currentHpBonus * PlayerController.instance.GetHealth() +
            maxHpBonus * PlayerController.instance.playerAttributes.maxHitPoints);
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(currentHpBonus),
            FormatPercentage(maxHpBonus)
        };
    }
}