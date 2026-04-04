using UnityEngine;

public class Magician_Future : Future_TarotCard
{
    int roomSkillCount = 0;

    int totalSkillCount = 0;

    public const int roomSkillGoal = 4;
    public const int totalSkillGoal = 15;

    public const int uses = 3;
    public const float cd = 10f;

    public Magician_Future(int q) : base(q)
    {
        cardName = "Magician_Future";
        reward = new Magician_Reward();
        arcana = Arcana.Magician;
        
        GetLocalizedDesc(uses, cd);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        GameManager.OnRoomChange += OnRoomChange;
        Future_Reward.OnSkillUsed += OnSkillUse;
    }

    protected override void RemoveListeners()
    {
        GameManager.OnRoomChange -= OnRoomChange;
        Future_Reward.OnSkillUsed -= OnSkillUse;
    }

    private void OnRoomChange()
    {
        roomSkillCount = 0;
        RefreshDescription();
    }

    private void OnSkillUse()
    {
        ++roomSkillCount;
        ++totalSkillCount;

        RefreshDescription();

        if (roomSkillCount >= roomSkillGoal || totalSkillCount >= totalSkillGoal)
        {
            CompleteQuest();
        }
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] { roomSkillCount, roomSkillGoal, totalSkillCount, totalSkillGoal };
    }
}