using UnityEngine;

public class PhysicalDmgBonus_Effect : Flat_Effects
{
    public PhysicalDmgBonus_Effect(float duration, float effectFlat) : base(duration, effectFlat)
    {
        effectStat = Stat.PhysicalDamageBonus;
        isDebuff = false;
        effectApplication = Application.Additive;
        hasVfx = false;
    }
}