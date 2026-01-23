using UnityEngine;

public class LoversPast_Effect : Effects
{
    public LoversPast_Effect() : base(-1)
    {
        effectStat = Stat.PastTarot;
        isDebuff = false;
        effectApplication = Application.Additive;
        hasVfx = true;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        // player-only effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        playerAttributes.spiritualAdditionalDmg += Lovers_Past.spiritualDmgBonus;
    }
}