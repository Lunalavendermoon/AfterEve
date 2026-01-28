using UnityEngine;

public class HermitPast_Effect : Effects
{
    public HermitPast_Effect() : base(-1)
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
        playerAttributes.hermitPast = true;
        playerAttributes.totalSpiritualVision += Hermit_Past.spiritVisionBonus;
    }
}