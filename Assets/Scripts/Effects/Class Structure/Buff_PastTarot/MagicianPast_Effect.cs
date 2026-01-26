public class MagicianPast_Effect : Effects
{
    public MagicianPast_Effect() : base(Magician_Past.coinBuffDuration)
    {
        effectStat = Stat.PastTarot;
        isDebuff = false;
        effectApplication = Application.Disable;
        hasVfx = false;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        // player-only effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        playerAttributes.magicianPastBonusCoin = true;
    }
}