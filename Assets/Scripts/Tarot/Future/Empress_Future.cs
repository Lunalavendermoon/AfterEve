public class Empress_Future : Future_TarotCard
{
    public const double healPercentGoal = 2f;
    public const int roomGoal = 4;

    private int maxHp;

    private double healCount = 0f;
    private int roomCount = 0;

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
        healCount += (double)amount / maxHp;

        if (healCount >= healPercentGoal)
        {
            CompleteQuest();
        }
    }

    private void OnRoomChange()
    {
        if (((double)PlayerController.instance.GetHealth()) / PlayerController.instance.playerAttributes.maxHitPoints >= 0.8f)
        {
            ++roomCount;

            if (roomCount >= roomGoal)
            {
                CompleteQuest();
            }
        }
    }

    public override string GetQuestText()
    {
        return $"heal {(int)(healCount * 100)}/{(int)(healPercentGoal * 100)}% Max HP " +
                    $"OR end {roomCount}/{roomGoal} combat rooms at 80% or more health";
    }
}
