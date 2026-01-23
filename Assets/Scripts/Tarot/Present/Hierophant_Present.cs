using UnityEngine;

public class Hierophant_Present : Present_TarotCard
{
    int[] enemiesChained = { 3, 4, 5, 6, 7 };
    int[] distance = { 5, 6, 7, 8, 9 };
    float[] chainTime = { 5f, 5f, 5f, 5f, 5f };
    float[] chainDmg = { .3f, .35f, .4f, .45f, .5f };
    float[] shieldIncrease = { .01f, .02f, .03f, .04f, .05f };

    public Hierophant_Present(int q) : base(q)
    {
        cardName = "Hierophant_Present";
        arcana = Arcana.Hierophant;
        
        PlayerController.instance.playerAttributes.chainDmg = chainDmg[level];
        PlayerController.instance.playerAttributes.enemiesChained = enemiesChained[level];
        PlayerController.instance.playerAttributes.chainRadius = distance[level];
        PlayerController.instance.playerAttributes.chainShieldIncrease = shieldIncrease[level];
        PlayerController.instance.playerAttributes.chainTime = chainTime[level];

    }
}
