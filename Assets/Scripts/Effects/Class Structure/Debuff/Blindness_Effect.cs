public class Blindness_Effect : Effects
{
    /// <summary>
    /// Player is unable to use Spiritual Vision, visibility is limited to a small space around the player,
    /// Enemy behaves as if player is not within its vision
    /// </summary>
    /// <param name="duration"></param>
    public Blindness_Effect(float duration) : base(duration)
    {
        effectStat = Stat.SpiritualVision;
        isDebuff = true;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool _)
    {
        entityAttributes.isBlind = true;
    }
}
