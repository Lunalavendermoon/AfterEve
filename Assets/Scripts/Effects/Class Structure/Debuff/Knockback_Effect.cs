using UnityEngine;

public class Knockback_Effect : Effects
{
    /// <summary>
    /// Unable to move, dash, or attack
    /// </summary>
    /// <param name="duration"></param>
    public Knockback_Effect(float duration) : base(duration)
    {
        effectStat = Stat.Knockback;
        isDebuff = true;
        effectApplication = Application.Disable;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        // TODO make sure knockback triggers when enemy attacks player
        playerAttributes.hasKnockback = true;
    }
}
