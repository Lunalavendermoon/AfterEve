using UnityEngine;

public class Shield_Effect : Flat_Effects
{
    /// <summary>
    /// Shield is increased by a flat number
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="duration"></param>
    /// <param name="effectFlat"></param>
    public Shield_Effect(PlayerAttributes attributes, float duration, int effectFlat) : base(attributes, duration, effectFlat)
    {
        effectStat = Stat.Shield;
        isDebuff = false;
        effectApplication = Application.Flat;
    }
}
