using UnityEngine;

public class Chariot_Present : Present_TarotCard
{
    float[] fireRateDecrease = {.5f, .45f, .4f, .35f, .3f};
    int[] extraBullets = {3,4,4,5,5};
    float[] baseDamageDecrease = {.4f, .35f, .3f, .25f, .2f};

    string knockbackRange = "0.5";

    public Chariot_Present(int q) : base(q)
    {
        cardName = "Chariot_Present";
        arcana = Arcana.Chariot;

        effects.Add(new FireRate_Effect(-1, 1f - fireRateDecrease[level]));
        effects.Add(new Strength_Effect(-1, 1f - baseDamageDecrease[level]));
        PlayerController.instance.playerAttributes.bullets += extraBullets[level];

        //TODO add AOE pulse
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();

        SetTableEntries("Chariot");
        
        SetDescriptionValues();
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
