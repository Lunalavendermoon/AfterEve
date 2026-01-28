using UnityEngine;

public class HierophantPast_Effect : Effects
{
    public HierophantPast_Effect() : base(-1)
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
        playerAttributes.spiritualDefense += Hierophant_Past.spiritualDefense;
    }
}