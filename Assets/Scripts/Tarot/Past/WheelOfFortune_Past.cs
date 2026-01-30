using UnityEngine;

public class WheelOfFortune_Past : Past_TarotCard
{
    PlayerEffectManager effectManager;
    public const float increaseAmt = 1.2f;

    EffectInstance ei = null;

    public WheelOfFortune_Past(int q) : base(q)
    {
        cardName = "WheelOfFortune_Past";
        arcana = Arcana.WheelOfFortune;
    }

    protected override void ApplyListenersEffects()
    {
        effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();

        GameManager.OnRoomChange += OnNewRoom;
    }

    // TODO: same issue as Hierophant_Past
    void OnNewRoom()
    {
        ApplyEffect();
    }

    void ApplyEffect()
    {
        if (ei != null)
        {
            effectManager.RemoveEffect(ei);
        }
        // TODO: effects seemingly not getting added despite debug & GetInstanceID appearing to be correct
        // (no VFX & strength doesn't increase damage dealt)
        ei = effectManager.AddBuff(GenRandomEffect());
    }

    Effects GenRandomEffect()
    {
        // TODO: add more buff types?
        switch (Random.Range(0, 3))
        {
            case 0:
                Debug.Log("Wheel of Fortune past - apply blessed effect");
                return new Blessed_Effect(increaseAmt, -1);
            case 1:
                Debug.Log("Wheel of Fortune past - apply fortified effect");
                return new Fortified_Effect(increaseAmt, -1);
            case 2:
                Debug.Log("Wheel of Fortune past - apply strength effect");
                return new Strength_Effect(increaseAmt, -1);
            default:
                Debug.Log("Wheel of Fortune past - generated invalid effect type, applying blessed effect");
                return new Blessed_Effect(increaseAmt, -1);
        }
    }
}