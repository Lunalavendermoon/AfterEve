public class EmperorPast_Effect : Effects
{
    public EmperorPast_Effect() : base(-1)
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
        playerAttributes.physicalAdditionalDmg += Emperor_Past.damageBonus;
        playerAttributes.basicDefense += Emperor_Past.defBonus;
    }
}