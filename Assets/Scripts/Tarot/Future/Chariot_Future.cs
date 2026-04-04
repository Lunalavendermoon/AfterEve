using UnityEngine;

public class Chariot_Future : Future_TarotCard
{
    public const float distGoal = 200f;
    public const int dashGoal = 40;

    private Vector3 oldPos;
    private float distCount = 0f;
    private int dashCount = 0;

    public const int uses = 3;
    public const float cd = 10f;

    public Chariot_Future(int q) : base(q)
    {
        cardName = "Chariot_Future";
        reward = new Chariot_Reward();
        arcana = Arcana.Chariot;
        
        GetLocalizedDesc(uses, cd);
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        oldPos = PlayerController.instance.gameObject.transform.position;
        Player_Dash.OnDash += OnDash;
        Player_Dash.OnDisplaced += OnMove;
        Player_Move.OnDisplaced += OnMove;
        GameManager.OnRoomChange += OnRoomChange;
    }

    protected override void RemoveListeners()
    {
        Player_Dash.OnDash -= OnDash;
        Player_Dash.OnDisplaced -= OnMove;
        Player_Move.OnDisplaced -= OnMove;
        GameManager.OnRoomChange -= OnRoomChange;
    }

    void OnRoomChange()
    {
        oldPos = PlayerController.instance.gameObject.transform.position;
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

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] { Mathf.RoundToInt(distCount), Mathf.RoundToInt(distGoal),
            dashCount, dashGoal };
    }
}