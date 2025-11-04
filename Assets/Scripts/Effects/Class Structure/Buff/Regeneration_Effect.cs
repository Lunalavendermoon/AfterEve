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
    }

    public override void ApplyEffect(EntityAttributes entityAttributes)
    {
        if (Time.time - startTime > incrementInterval || initialApplication)
        {
            // TODO: recover HP to the target entity
            float regenAmount = entityAttributes.hitPoints * effectRate;
            Debug.Log("Regenerated amount: " + regenAmount);
            startTime = Time.time;
            initialApplication = false;
        }
    }
}
