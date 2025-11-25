using UnityEngine;

public class HitCountShield_Effect : Effects
{
    public int amount;
    bool initialApplication = true;

    /// <summary>
    /// Shield that can completely absorb hits
    /// </summary>
    /// <param name="duration"></param>
    public HitCountShield_Effect(int amount, float duration) : base(duration)
    {
        effectStat = Stat.HitCountShield;
        isDebuff = false;
        effectApplication = Application.Flat;

        this.amount = amount;
    }

    public override void ApplyEffect(EntityAttributes _e, bool _)
    {
        // player-exclusive effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool _)
    {
        if (initialApplication)
        {
            PlayerController.instance.GainHitCountShield(amount);
            initialApplication = false;
        }
    }
}
