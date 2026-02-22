using UnityEngine;

public class Chariot_Present : Present_TarotCard
{
    float[] fireRateDecrease = {.5f, .45f, .4f, .35f, .3f};
    int[] extraBullets = {3,4,4,5,5};
    float[] baseDamageDecrease = {.4f, .35f, .3f, .25f, .2f};

    public Chariot_Present(int q) : base(q)
    {
        cardName = "Chariot_Present";
        arcana = Arcana.Chariot;

        effects.Add(new FireRate_Effect(-1, fireRateDecrease[level]));
        effects.Add(new Strength_Effect(-1, baseDamageDecrease[level]));
        PlayerController.instance.playerAttributes.bullets += extraBullets[level];

        //TODO add AOE pulse
    }

    protected override void GetLocalizedDesc()
    {
        // TODO finish this
        base.GetLocalizedDesc();
        desc.TableEntryReference = "FoolPresent";
        desc.Arguments = new object[] { "temp", "temp" };
    }
}
