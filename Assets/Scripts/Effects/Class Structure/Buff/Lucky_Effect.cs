public class Lucky_Effect : Flat_Effects
{
    /// <summary>
    /// Luck is increased by a flat number
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectFlat"></param>
    public Lucky_Effect(float duration, int effectFlat) : base(duration, effectFlat)
    {
        effectStat = Stat.Luck;
        isDebuff = false;
        effectApplication = Application.Flat;
    }
}
