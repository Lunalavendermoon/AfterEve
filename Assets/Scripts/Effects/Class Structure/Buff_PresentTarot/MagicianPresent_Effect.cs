public class MagicianPresent_Effect : Effects
{
    int numBounces;
    float bulletBounceDmgDecrease;

    public MagicianPresent_Effect(int numBounces, float bulletBounceDmgDecrease) : base(-1)
    {
        this.numBounces = numBounces;
        this.bulletBounceDmgDecrease = bulletBounceDmgDecrease;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        // player-only effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool increment)
    {
        playerAttributes.bulletBounces = numBounces;
        playerAttributes.bulletBounceDmgDecrease = bulletBounceDmgDecrease;
    }
}