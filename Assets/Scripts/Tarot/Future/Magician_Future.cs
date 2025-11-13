public class Magician_Future : Future_TarotCard
{
    int roomSkillCount = 0;

    int totalSkillCount = 0;

    public const int roomSkillGoal = 4;
    public const int totalSkillGoal = 15;

    public Magician_Future(string s, int q) : base(s, q)
    {
        reward = new Magician_Reward(this);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        // TODO add room change listener
    }

    protected override void RemoveListeners()
    {
        // TODO remove room change listener
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
}