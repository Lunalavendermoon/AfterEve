using UnityEngine;

public class ChariotPast_Effect : Effects
{
    public ChariotPast_Effect() : base(-1)
    {
        effectStat = Stat.PastTarot;
        isDebuff = false;
        effectApplication = Application.Additive;
        hasVfx = false;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        // player-only effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        playerAttributes.damageDealtBonus += Chariot_Past.damageBonus;
    }
}