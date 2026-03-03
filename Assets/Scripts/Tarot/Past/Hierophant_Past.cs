using UnityEngine;

public class Hierophant_Past : Past_TarotCard
{
    public const float shieldPercentage = 0.2f;
    public const int spiritualDefense = 20;

    public Hierophant_Past(int q) : base(q)
    {
        cardName = "Hierophant_Past";
        arcana = Arcana.Hierophant;
    }

    protected override void ApplyListenersEffects()
    {
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddBuff(new HierophantPast_Effect());

        GameManager.OnRoomChange += OnNewRoom;
    }

    void OnNewRoom()
    {
        ApplyShield();
    }

    void ApplyShield()
    {
        PlayerController.instance.GainRegularShield((int)(shieldPercentage * PlayerController.instance.playerAttributes.maxHitPoints));
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Hierophant");

        SetDescriptionValues();
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