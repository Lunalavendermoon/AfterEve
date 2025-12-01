using UnityEngine;

public class HighPriestess_Present : Present_TarotCard
{
    float[] applyProbability = {.25f, .3f, .4f, .5f, .6f};
    float[] spiritualApplyProbability = {.5f, .6f, .7f, .8f, 1};
    float weakPercent = .2f;
    
    PlayerGun playerGun;

    public HighPriestess_Present(int q) : base(q)
    {
        name = "HighPriestess_Present";

        // TODO make effect only apply sometimes
        playerGun = PlayerController.instance.gameObject.GetComponent<PlayerGun>();
        playerGun.AddEffect(new Weak_Effect(3, weakPercent));
    }

}
