public class Empress_Future : Future_TarotCard
{
    public const double healPercentGoal = 2f;
    public const int roomGoal = 4;

    private int maxHp;

    private double healCount = 0f;
    private int roomCount = 0;

    public Empress_Future(string s, int q) : base(s, q)
    {
        reward = new Empress_Reward(this);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        PlayerController.OnHealed += OnPlayerHeal;
        maxHp = PlayerController.instance.playerAttributes.maxHitPoints;
        // TODO add finish combat room listener
    }

    protected override void RemoveListeners()
    {
        PlayerController.OnHealed -= OnPlayerHeal;
        // TODO remove finish combat room listener
    }

    private void OnPlayerHeal(int amount)
    {
        healCount += (double)amount / maxHp;

        if (healCount >= healPercentGoal)
        {
            CompleteQuest();
        }
    }

    private void OnRoomChange()
    {
        PlayerAttributes attr = PlayerController.instance.playerAttributes;
        if (((double)attr.hitPoints) / attr.maxHitPoints >= 0.8f)
        {
            ++roomCount;

            if (roomCount >= roomGoal)
            {
                CompleteQuest();
            }
        }
    }
}
