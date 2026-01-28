using UnityEngine;

public class Fortified_Additive_Effect : Flat_Effects
{
    /// <summary>
    /// Basic Defense is increased by a flat amount before multpliers are applied
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectFlat"></param>
    public Fortified_Additive_Effect(float duration, int effectFlat) : base(duration, effectFlat)
    {
        effectStat = Stat.BasicDefense;
        isDebuff = false;
        effectApplication = Application.Additive;
        iconType = IconType.BuffFortify;
        hasVfx = true;
    }
}
