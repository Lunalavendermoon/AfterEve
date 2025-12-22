using UnityEngine;

public class Lovers_Present : Present_TarotCard
{
    float[] basicDamage = { .1f, .2f, .3f, .4f, .5f };
    float[] spiritualDamage = { .1f, .2f, .3f, .4f, .5f };
    float[] strengthBuff = { .2f, .25f, .3f, .35f, .4f };

    public Lovers_Present(int q) : base(q)
    {
        name = "Lovers_Present";
        PlayerController.instance.CreateClone();

        //TODO: clone attack functionality
    }
}
