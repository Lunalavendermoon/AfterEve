public class Shield_Effect : Flat_Effects
{
    /// <summary>
    /// Shield is increased by a flat number
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectFlat"></param>
    public Shield_Effect(float duration, int effectFlat) : base(duration, effectFlat)
    {
        effectStat = Stat.Shield;
        isDebuff = false;
        effectApplication = Application.Flat;
    }

    public override void ApplyPlayerEffect(PlayerAttributes playerAttributes)
    {
        // unlike other flat effects, we only want to apply the shield a single time
        if (initialApplication)
        {
            playerAttributes.shield += (int)effectRate;
            initialApplication = false;
        }
    }
}
