using UnityEngine;

public class Magician_Past : Past_TarotCard
{
    const int roomChangeCoins = 5;
    public const int bonusCoinAmount = 1;
    public const float coinBuffDuration = 10f;

    public Magician_Past() : base()
    {
        cardName = "Magician_Past";
        arcana = Arcana.Magician;

        GetLocalizedDesc();
    }

    protected override void ApplyListenersEffects(bool muted = false)
    {
        GameManager.OnRoomChange += OnRoomChange;
        Future_Reward.OnSkillUsed += OnSkillUsed;
    }

    protected override void RemoveListeners(bool muted = false)
    {
        GameManager.OnRoomChange -= OnRoomChange;
        Future_Reward.OnSkillUsed -= OnSkillUsed;
    }

    void OnRoomChange()
    {
        PlayerController.instance.ChangeCoins(roomChangeCoins);
    }

    void OnSkillUsed()
    {
        // we trust that Effect Manager handles timers and stacking correctly... :D
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddBuff(new MagicianPast_Effect());
    }
    
    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            roomChangeCoins,
            bonusCoinAmount,
            Rnd(coinBuffDuration)
        };
    }
}