using UnityEngine;

public class FoolPast_Effect : Effects
{
    public FoolPast_Effect() : base(-1)
    {
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        // player-only effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        playerAttributes.physicalAdditionalDmg += Fool_Past.physicalDmgBonus;
    }
}