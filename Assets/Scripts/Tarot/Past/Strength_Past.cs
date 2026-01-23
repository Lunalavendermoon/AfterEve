using UnityEngine;

public class Strength_Past : Past_TarotCard
{
    public const float resistanceBonus = 10f;

    public Strength_Past(int q) : base(q)
    {
        cardName = "Strength_Past";
        arcana = Arcana.Strength;
    }

    protected override void ApplyListenersEffects()
    {
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddBuff(new StrengthPast_Effect());
    }
}