public class EmperorPresent_Effect : Effects
{
    public EmperorPresent_Effect() : base(-1)
    {
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        // player-only effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        playerAttributes.emperorPresent = true;
    }
}