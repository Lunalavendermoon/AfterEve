using UnityEngine;

public class Regular_Shield : Shield
{
    public Regular_Shield(int amount, float duration) : base(amount, ShieldType.Regular, duration)
    {
    }
    public override int TakeShieldDamage(int amount)
    {
        if (amount <= shieldAmount)
        {
            shieldAmount -= amount;
            return 0;
        }
        int diff = amount - shieldAmount;
        shieldAmount = 0;
        return diff;
    }
}