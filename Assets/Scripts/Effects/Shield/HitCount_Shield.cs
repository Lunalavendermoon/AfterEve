public class HitCount_Shield : Shield
{
    public HitCount_Shield(int amount, float duration) : base(amount, ShieldType.HitCount, duration)
    {
    }
    public override int TakeShieldDamage(int _)
    {
        --shieldAmount;
        return 0;
    }
}