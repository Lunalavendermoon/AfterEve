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

    // TODO: Slight issue w/ this not getting triggered in the first room, but i think that's because
    // the map is getting initialized earlier than the hierophant past card is added to the player
    // so this hopefully shouldn't be a problem once the full game is wired up & the past tarot is created before map generates
    void OnNewRoom()
    {
        PlayerController.instance.GainRegularShield((int)(shieldPercentage * PlayerController.instance.playerAttributes.maxHitPoints));
    }
}