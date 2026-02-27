using UnityEngine;

public class Lovers_Present : Present_TarotCard
{
    float[] basicDamage = { 1.1f, 1.2f, 1.3f, 1.4f, 1.5f };
    float[] spiritualDamage = { 1.1f, 1.2f, 1.3f, 1.4f, 1.5f };
    float[] strengthBuff = { 1.2f, 1.25f, 1.3f, 1.35f, 1.4f };

    string markDuration = "3";

    public Lovers_Present(int q) : base(q)
    {
        cardName = "Lovers_Present";
        arcana = Arcana.Lovers;
        
        PlayerController.instance.CreateClone(basicDamage[level], spiritualDamage[level], strengthBuff[level]);

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
            FormatPercentage(basicDamage[level]),
            FormatPercentage(spiritualDamage[level]),
            markDuration,
            FormatPercentage(strengthBuff[level])
        };
    }
}
