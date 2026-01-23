using UnityEngine;

public class Lovers_Past : Past_TarotCard
{
    public const float spiritualDmgBonus = 0.2f;

    public Lovers_Past(int q) : base(q)
    {
        cardName = "Lovers_Past";
        arcana = Arcana.Lovers;
    }

    protected override void ApplyListeners()
    {
        // technically not a listener, but idk where to put this lol
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddEffect(new LoversPast_Effect());
    }
}