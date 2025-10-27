using UnityEngine;

public class Fortified_Flat_Effect : Flat_Effects
{
    /// <summary>
    /// Basic Defense is increased by a flat amount after multpliers are applied
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectFlat"></param>
    public Fortified_Flat_Effect(float duration, int effectFlat) : base(duration, effectFlat)
    {
        effectStat = Stat.BasicDefense;
        isDebuff = false;
        effectApplication = Application.Flat;
    }
}
