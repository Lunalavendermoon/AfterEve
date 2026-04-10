using UnityEngine;

public class Hierophant_Past : Past_TarotCard
{
    public const float shieldPercentage = 0.2f;
    public const int spiritualDefense = 20;

    public Hierophant_Past() : base()
    {
        effects.Add(new HierophantPast_Effect());

        cardName = "Hierophant_Past";
        arcana = Arcana.Hierophant;

        GetLocalizedDesc();
    }

    protected override void ApplyListenersEffects(bool muted = false)
    {
        GameManager.OnRoomChange += OnNewRoom;
    }

    protected override void RemoveListeners(bool muted = false)
    {
        GameManager.OnRoomChange -= OnNewRoom;
    }

    void OnNewRoom()
    {
        ApplyShield();
    }

    void ApplyShield()
    {
        PlayerController.instance.GainRegularShield((int)(shieldPercentage * PlayerController.instance.playerAttributes.maxHitPoints));
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(shieldPercentage),
            spiritualDefense
        };
    }
}