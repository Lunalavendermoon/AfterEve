using UnityEngine;

public class Pierce_Effect : Effects
{
    /// <summary>
    /// Ignore Basic Defense
    /// </summary>
    /// <param name="duration"></param>
    public Pierce_Effect(float duration) : base(duration)
    {
        effectStat = Stat.Damage;
        isDebuff = false;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        playerAttributes.ignoreBasicDef = true;
    }
}
