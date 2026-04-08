using UnityEngine;

public class Strength_Present : Present_TarotCard
{
    float[] damageReducePierce = {.3f, .25f, .2f, .15f, .1f};
    float[] sunderedPercent = {.2f, .25f, .3f, .35f, .4f};
    float[] additionalDmg = {2f, 2.5f, 3f, 3.5f, 4f};

    float sunderDuration = 5f;

    float chargeAttackTime = 2f;

    public Strength_Present(int q) : base(q)
    {
        cardName = "Strength_Present";
        arcana = Arcana.Strength;

        AddNewLevelEffects();

        // TODO Sundered ability ;-;

        GetLocalizedDesc();
    }

    protected override void AddNewLevelEffects()
    {
        effects.Add(new StrengthPresent_Effect(damageReducePierce[level]));
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(damageReducePierce[level]),
            "100",
            Rnd(chargeAttackTime),
            FormatPercentage(sunderedPercent[level]),
            Rnd(sunderDuration),
            FormatPercentage(additionalDmg[level])
        };
    }
}
