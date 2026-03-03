using UnityEngine;

public class Hermit_Present : Present_TarotCard
{
    float[] ammoCapacityDecrease = { 0.5f, 0.4f, 0.3f, 0.2f, 0.1f };
    float[] fireRateDecrease = { 0.5f, 0.45f, 0.4f, 0.35f, 0.3f };
    float[] additionalDamage = { 1.5f, 1.7f, 1.9f, 2.1f, 2.3f };
    float[] weakpointDamage = { 0.2f, 0.3f, 0.4f, 0.5f, 0.6f };

    public Hermit_Present(int q) : base(q)
    {
        cardName = "Hermit_Present";
        arcana = Arcana.Hermit;

        // TODO apply card effects
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
            FormatPercentage(ammoCapacityDecrease[level]),
            FormatPercentage(fireRateDecrease[level]),
            FormatPercentage(additionalDamage[level]),
            FormatPercentage(weakpointDamage[level])
        };
    }
}
