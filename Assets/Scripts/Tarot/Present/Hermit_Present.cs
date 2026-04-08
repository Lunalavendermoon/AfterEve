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

        AddNewLevelEffects();

        // TODO handle weak point stuff (see tarot card docs)

        GetLocalizedDesc();
    }

    protected override void AddNewLevelEffects()
    {
        effects.Add(new FireRate_Effect(-1, 1f - ammoCapacityDecrease[level]));
        effects.Add(new AmmoCapacity_MultEffect(-1, 1f - fireRateDecrease[level]));
        effects.Add(new PhysicalDmgBonus_Effect(-1, additionalDamage[level]));
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
