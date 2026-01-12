using UnityEngine;

public class Magician_Present : Present_TarotCard
{
    int[] bounceNum = {3,3,4,4,5};
    float[] damageReducedPerBounce = {.3f, .25f, .2f, .1f, 0f};
    
    public Magician_Present(int q) : base(q)
    {
        PlayerController.instance.playerAttributes.bulletBounces = bounceNum[level];
        PlayerController.instance.playerAttributes.bulletBounceDmgDecrease = damageReducedPerBounce[level];
        cardName = "Magician_Present";
    }
}
