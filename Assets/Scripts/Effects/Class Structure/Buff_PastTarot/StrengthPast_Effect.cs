using UnityEngine;

public class StrengthPast_Effect : Effects
{
    public StrengthPast_Effect() : base(-1)
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
        playerAttributes.resistance += Strength_Past.resistanceBonus;
    }
}