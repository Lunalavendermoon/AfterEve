public class Slow_Effect : Multiplier_Effects
{
    /// <summary>
    /// Reduce Speed by a percentage per second
    /// </summary>
    /// <param name="duration"> duration of time (seconds) the effect lasts for </param>
    /// <param name="healthDecreasePercent"> lose this percent of HP each increment </param>
    /// <param name="timeBetweenIncrements"> time between each increment </param>
    public Slow_Effect(float duration, float effectMultiplier) : base(duration, effectMultiplier)
    {
        // facts
        effectStat = Stat.Speed;
        isDebuff = true;
        isIncremental = true;
        incrementInterval = 1;
    }
}
