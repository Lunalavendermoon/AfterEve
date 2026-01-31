public class Magician_Future : Future_TarotCard
{
    int roomSkillCount = 0;

    int totalSkillCount = 0;

    public const int roomSkillGoal = 4;
    public const int totalSkillGoal = 15;

    public Magician_Future(int q) : base(q)
    {
        cardName = "Magician_Future";
        reward = new Magician_Reward(this);
        arcana = Arcana.Magician;
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
    }

    private void OnSkillUse()
    {
        ++roomSkillCount;
        ++totalSkillCount;

        if (roomSkillCount >= roomSkillGoal || totalSkillCount >= totalSkillGoal)
        {
            CompleteQuest();
        }
    }

    public override string GetQuestText()
    {
        return $"use skills {roomSkillCount}/{roomSkillGoal} times in one room " +
                $"OR use skills {totalSkillCount}/{totalSkillGoal} times";
    }
}