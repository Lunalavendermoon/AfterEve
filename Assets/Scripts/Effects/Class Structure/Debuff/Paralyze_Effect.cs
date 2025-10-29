using UnityEngine;

public class Paralyze_Effect : Effects
{
    /// <summary>
    /// Unable to move, dash, or attack
    /// </summary>
    /// <param name="duration"></param>
    public Paralyze_Effect(float duration) : base(duration)
    {
        effectStat = Stat.Movement;
        isDebuff = true;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes)
    {
        entityAttributes.isParalyzed = true;
    }
}
