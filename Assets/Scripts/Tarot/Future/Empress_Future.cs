using UnityEngine;

public class Empress_Future : Future_TarotCard
{
    public const float healPercentGoal = 2f;
    public const int roomGoal = 4;

    private int maxHp;

    private float healCount = 0f;
    private int roomCount = 0;
    const float roomGoalHPThreshold = 0.8f;

    public const int uses = 5;
    public const float cd = 5f;

    public Empress_Future(int q) : base(q)
    {
        cardName = "Empress_Future";
        reward = new Empress_Reward(this);
        arcana = Arcana.Empress;
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        PlayerController.OnHealed += OnPlayerHeal;
        maxHp = PlayerController.instance.playerAttributes.maxHitPoints;
        GameManager.OnCombatRoomClear += OnRoomChange;
    }

    protected override void RemoveListeners()
    {
        PlayerController.OnHealed -= OnPlayerHeal;
        GameManager.OnCombatRoomClear -= OnRoomChange;
    }

    private void OnPlayerHeal(int amount)
    {
        healCount += (float)amount / maxHp;
        RefreshDescription();

        if (healCount >= healPercentGoal)
        {
            CompleteQuest();
        }
    }

    private void OnRoomChange()
    {
        if (((double)PlayerController.instance.GetHealth()) / PlayerController.instance.playerAttributes.maxHitPoints >=
                                                                                                        roomGoalHPThreshold)
        {
            ++roomCount;
            RefreshDescription();

            if (roomCount >= roomGoal)
            {
                CompleteQuest();
            }
        }
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Empress");

        rewardDesc.Arguments = new object[] { Mathf.RoundToInt(Empress_Reward.healPercent * 100),
            Empress_Reward.pulseDuration, Mathf.RoundToInt(cd), uses };

        SetDescriptionValues();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] { Mathf.RoundToInt(healCount * 100),
            Mathf.RoundToInt(healPercentGoal * 100), roomCount, roomGoal,
            Mathf.RoundToInt(roomGoalHPThreshold * 100) };
    }
}
