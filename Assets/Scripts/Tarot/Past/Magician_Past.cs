using UnityEngine;

public class Magician_Past : Past_TarotCard
{
    const int roomChangeCoins = 5;
    public const int bonusCoinAmount = 1;
    public const float coinBuffDuration = 10f;

    public Magician_Past(int q) : base(q)
    {
        cardName = "Magician_Past";
        arcana = Arcana.Magician;
    }

    protected override void ApplyListenersEffects()
    {
        GameManager.OnRoomChange += OnRoomChange;
        Future_Reward.OnSkillUsed += OnSkillUsed;
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
}