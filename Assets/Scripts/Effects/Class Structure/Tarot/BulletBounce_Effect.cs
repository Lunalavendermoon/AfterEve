public class BulletBounce_Effect : Effects
{
    public BulletBounce_Effect(float duration) : base(duration)
    {
        effectStat = Stat.BulletBounce;
        isDebuff = false;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool _)
    {
        // player-only effect
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes, bool _)
    {
        playerAttributes.hasBulletBounce = true;
    }
}
