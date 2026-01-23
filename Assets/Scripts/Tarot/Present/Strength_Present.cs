using UnityEngine;

public class Strength_Present : Present_TarotCard
{
    float[] damageReducePierce = {.3f, .25f, .2f, .15f, .1f};
    float[] sunderedPercent = {.2f, .25f, .3f, .35f, .4f};
    float[] additionalDmg = {2f, 2.5f, 3f, 3.5f, 4f};

    public Strength_Present(int q) : base(q)
    {
        cardName = "Strength_Present";
        arcana = Arcana.Strength;

        PlayerAttributes attributes = PlayerController.instance.playerAttributes;
        attributes.bulletPierces = 3; // TODO change this
        attributes.bulletBounceDmgDecrease = .3f;

        // Sundered ability ;-;
    }
}
