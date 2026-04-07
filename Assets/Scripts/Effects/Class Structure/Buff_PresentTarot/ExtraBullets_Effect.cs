public class ExtraBullets_Effect : Flat_Effects
{
    public ExtraBullets_Effect(float duration, int effectFlat) : base(duration, effectFlat)
    {
        effectStat = Stat.NumBullets;
        isDebuff = false;
        isIncremental = false;
    }
}