using UnityEngine;

public class HighPriestess_Present : Present_TarotCard
{
    float[] applyProbability = {.25f, .3f, .4f, .5f, .6f};
    float[] spiritualApplyProbability = {.5f, .6f, .7f, .8f, 1};
    float weakPercent = .2f;

    int normalDuration = 3;
    int spiritualDuration = 5;
    
    PlayerGun playerGun;

    public HighPriestess_Present(int q) : base(q)
    {
        cardName = "HighPriestess_Present";
        arcana = Arcana.HighPriestess;

        // TODO make effect only apply sometimes
        playerGun = PlayerController.instance.gameObject.GetComponent<PlayerGun>();
        playerGun.AddEffect(new Weak_Effect(3, weakPercent));
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();

        SetTableEntries("HighPriestess");
        
        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] {
            FormatPercentage(applyProbability[level]),
            normalDuration,
            FormatPercentage(weakPercent),
            FormatPercentage(spiritualApplyProbability[level]),
            spiritualDuration
        };
    }
}
