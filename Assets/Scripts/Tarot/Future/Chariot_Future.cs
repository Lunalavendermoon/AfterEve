using UnityEngine;

public class Chariot_Future : Future_TarotCard
{
    public const double distGoal = 200f;
    public const int dashGoal = 40;

    private Vector2 oldPos;
    private double distCount = 0f;
    private double dashCount = 0;

    private bool countDist = true;

    public Chariot_Future(int q) : base(q)
    {
        name = "Chariot_Future";
        reward = new Chariot_Reward(this);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        oldPos = PlayerController.instance.gameObject.transform.position;
        Player_Dash.OnDash += OnDash;
        // TODO movement listener
        // TODO room change listener
    }

    protected override void RemoveListeners()
    {
        countDist = false;
        Player_Dash.OnDash -= OnDash;
        // TODO movement listener
        // TODO room change listener
    }

    void Update()
    {
        if (!countDist)
        {
            return;
        }
        // For now, considers any player displacement to be "traveling"
        // i.e. if player is knocked back by enemy, it still counts
        // we can refine this in the future if needed
        Vector2 newPos = PlayerController.instance.gameObject.transform.position;
        distCount += Vector2.Distance(oldPos, newPos);
        oldPos = newPos;

        if (distCount >= distGoal)
        {
            CompleteQuest();
        }
    }

    private void OnDash()
    {
        Debug.Log("Super sigma");
        ++dashCount;

        if (dashCount >= dashGoal)
        {
            CompleteQuest();
        }
    }
}