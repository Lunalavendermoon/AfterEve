using UnityEngine;

public class Hierophant_Future : Future_TarotCard
{
    public const float visionGoal = 30f;

    private float visionCount = 0f;

    private float startTime;

    public const int uses = 5;
    public const float cd = 10f;

    public Hierophant_Future(int q) : base(q)
    {
        cardName = "Hierophant_Future";
        reward = new Hierophant_Reward(this);
        arcana = Arcana.Hierophant;
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        PlayerController.OnSpiritualVisionChange += OnSpiritualVisionToggle;
    }

    protected override void RemoveListeners()
    {
        PlayerController.OnSpiritualVisionChange -= OnSpiritualVisionToggle;
    }

    private void OnSpiritualVisionToggle(bool isOn)
    {
        if (isOn)
        {
            startTime = Time.time;
        }
        else
        {
            visionCount += Time.time - startTime;
            RefreshDescription();
            if (visionCount >= visionGoal)
            {
                CompleteQuest();
            }
        }
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Hierophant");
        
        rewardDesc.Arguments = new object[] { Hierophant_Reward.shieldAmount,
            Mathf.RoundToInt(Hierophant_Reward.shieldDuration), Mathf.RoundToInt(cd), uses };

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] { Mathf.RoundToInt(visionCount), Mathf.RoundToInt(visionGoal) };
    }
}