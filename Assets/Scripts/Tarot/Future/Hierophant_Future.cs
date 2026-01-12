using UnityEngine;

public class Hierophant_Future : Future_TarotCard
{
    public const double visionGoal = 30f;
    // TODO add actual value once design finalizes
    public const int shieldGoal = 500;

    private double visionCount = 0f;
    private int shieldCount = 0;

    private double startTime;

    public Hierophant_Future(int q) : base(q)
    {
        cardName = "Hierophant_Future";
        reward = new Hierophant_Reward(this);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        PlayerController.OnSpiritualVisionChange += OnSpiritualVisionToggle;
        PlayerController.OnShielded += OnShielded;
    }

    protected override void RemoveListeners()
    {
        PlayerController.OnSpiritualVisionChange -= OnSpiritualVisionToggle;
        PlayerController.OnShielded -= OnShielded;
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
            if (visionCount >= visionGoal)
            {
                CompleteQuest();
            }
        }
    }

    private void OnShielded(int amount)
    {
        shieldCount += amount;
        if (shieldCount >= shieldGoal)
        {
            CompleteQuest();
        }
    }

    public override string GetQuestText()
    {
        return $"use spiritual vision for more than {(int)visionCount}/{(int)visionGoal} " +
                $"OR generate {shieldCount}/{shieldGoal} shield";
    }
}