public class Magician_Future : Future_TarotCard
{
    int currentRoomSkillCount = 0;

    int currentTotalSkillCount = 0;

    public const int roomSkillCount = 4;
    public const int totalSkillCount = 15;

    public Magician_Future(string s, int q) : base(s, q)
    {
        reward = new Magician_Reward();
    }

    void OnEnable()
    {
        // TODO add listener
    }

    void OnDisable()
    {
        // TODO remove listener
    }

    private void OnRoomChange()
    {
        currentRoomSkillCount = 0;
    }

    private void OnSkillUse()
    {
        ++currentRoomSkillCount;
        ++currentTotalSkillCount;

        // prevents reward from triggering twice due to a single skill use
        bool giveReward = false;

        if (currentRoomSkillCount >= roomSkillCount)
        {
            currentRoomSkillCount = 0;
            giveReward = true;
        }
        if (currentTotalSkillCount >= totalSkillCount)
        {
            currentTotalSkillCount = 0;
            giveReward = true;
        }

        if (giveReward)
        {
            RewardPlayer();
        }
    }
}