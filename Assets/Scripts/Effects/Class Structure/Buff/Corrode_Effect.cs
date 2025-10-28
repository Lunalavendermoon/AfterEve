using UnityEngine;

public class Corrode_Effect : Effects
{
    /// <summary>
    /// Ignore Basic Defense
    /// </summary>
    /// <param name="duration"></param>
    public Corrode_Effect(float duration) : base(duration)
    {
        effectStat = Stat.Damage;
        isDebuff = false;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        playerAttributes.ignoreSpiritualDef = true;
    }
}
