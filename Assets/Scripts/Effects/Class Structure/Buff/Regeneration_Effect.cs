using UnityEngine;

public class Regeneration_Effect : Multiplier_Effects
{
    /// <summary>
    /// HP regenerate by a percentage per second
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectMultiplyAdditive"></param>
    public Regeneration_Effect(float duration, float effectMultiplyAdditive) : base(duration, effectMultiplyAdditive)
    {
        effectStat = Stat.HP;
        isDebuff = false;
        isIncremental = true;
        incrementInterval = 1;

        effectApplication = Application.Multiplier;
    }

    public override void ApplyEffect(EntityAttributes entityAttributes, bool increment)
    {
        if (increment)
        {
            // TODO: recover HP to the target entity
            float regenAmount = entityAttributes.hitPoints * effectRate;
            Debug.Log("Regenerated amount: " + regenAmount);
            initialApplication = false;
        }
    }
}
