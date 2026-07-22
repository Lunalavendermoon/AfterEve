using UnityEngine;

public class Chariot_Present : Present_TarotCard
{
    float[] fireRateDecrease = {.5f, .45f, .4f, .35f, .3f};
    int[] extraBullets = {3,4,4,5,5};
    float[] baseDamageDecrease = {.6f, .55f, .5f, .45f, .4f};

    string knockbackRange = "0.5";

    public Chariot_Present(int q) : base(q)
    {
        cardName = "Chariot_Present";
        arcana = Arcana.Chariot;

        AddNewLevelEffects();

        PlayerController.instance.chariotPulseActive = true;

        GetLocalizedDesc();
    }

    protected override void AddNewLevelEffects()
    {
        effects.Add(new FireRate_Effect(-1, 1f - fireRateDecrease[level]));
        effects.Add(new DamageDeduction_Effect(-1, 1f - baseDamageDecrease[level]));
        effects.Add(new ExtraBullets_Effect(-1, extraBullets[level]));
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] {
            FormatPercentage(fireRateDecrease[level]),
            FormatPercentage(baseDamageDecrease[level]),
            extraBullets[level],
            knockbackRange
        };
    }
}
