public class Blindness_Effect : Effects
{
    /// <summary>
    /// Unable to use Spiritual Vision, visibility is limited to a small space around the player
    /// </summary>
    /// <param name="duration"></param>
    public Blindness_Effect(float duration) : base(duration)
    {
        effectStat = Stat.SpiritualVision;
        isDebuff = true;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        // TODO disable spiritual vision when isBlind is true
        playerAttributes.isBlind = true;
    }
}
