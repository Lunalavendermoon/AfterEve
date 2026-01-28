using UnityEngine;

public class Knockback_Effect : Effects
{
    /// <summary>
    /// Knocked back away from the source of damage
    /// </summary>
    /// <param name="duration"></param>
    public Knockback_Effect(float duration) : base(duration)
    {
        effectStat = Stat.Knockback;
        isDebuff = true;
        effectApplication = Application.Disable;

        iconType = IconType.DebuffKnockback;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool _)
    {
        // TODO make sure knockback triggers when enemy attacks player
        entityAttributes.hasKnockback = true;
    }
}
