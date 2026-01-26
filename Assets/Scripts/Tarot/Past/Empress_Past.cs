using UnityEngine;

public class Empress_Past : Past_TarotCard
{
    const float currentHpBonus = 0.05f;
    const float maxHpBonus = 0.03f;

    public Empress_Past(int q) : base(q)
    {
        cardName = "Empress_Past";
        arcana = Arcana.Empress;
    }

    protected override void ApplyListenersEffects()
    {
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddBuff(new EmpressPast_Effect());
    }

    public static int GetDamageBonus()
    {
        return (int)(currentHpBonus * PlayerController.instance.health +
            maxHpBonus * PlayerController.instance.playerAttributes.maxHitPoints);
    }
}