using UnityEngine;

public class Chariot_Future : Future_TarotCard
{
    public const double distGoal = 200f;
    public const int dashGoal = 40;

    private Vector3 oldPos;
    private double distCount = 0f;
    private double dashCount = 0;

    public Chariot_Future(int q) : base(q)
    {
        name = "Chariot_Future";
        reward = new Chariot_Reward(this);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        oldPos = PlayerController.instance.gameObject.transform.position;
        Player_Dash.OnDash += OnDash;
        Player_Dash.OnDisplaced += OnMove;
        Player_Move.OnDisplaced += OnMove;
        // TODO add room change listener (need to reset oldPos)
    }

    protected override void RemoveListeners()
    {
        Player_Dash.OnDash -= OnDash;
        Player_Dash.OnDisplaced -= OnMove;
        Player_Move.OnDisplaced -= OnMove;
        // TODO remove room change listener
    }

    void OnMove()
    {
        Vector3 newPos = PlayerController.instance.gameObject.transform.position;
        distCount += Vector3.Distance(oldPos, newPos);
        oldPos = newPos;

        if (distCount >= distGoal)
        {
            CompleteQuest();
        }
    }

    private void OnDash()
    {
        ++dashCount;

        if (dashCount >= dashGoal)
        {
            CompleteQuest();
        }
    }

    public override string GetQuestText()
    {
        return $"travel {(int)distCount}/{(int)distGoal} units OR dash {dashCount}/{dashGoal} times";
    }
}