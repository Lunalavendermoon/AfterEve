using UnityEngine;

public class Regeneration_Effect : Multiplier_Effects
{
    /// <summary>
    /// HP regenerate by a percentage per second
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="effectMultiplier"></param>
    public Regeneration_Effect(float duration, float effectMultiplier) : base(duration, effectMultiplier)
    {
        effectStat = Stat.HP;
        isDebuff = false;
        isIncremental = true;
        incrementInterval = 1;
    }

    public override void ApplyEffect(PlayerAttributes playerAttributes)
    {
        if (Time.time - startTime > incrementInterval || initialApplication)
        {
            // TODO: recover HP to the player
            float regenAmount = playerAttributes.hitPoints * effectRate;
            Debug.Log("Regenerated amount: " + regenAmount);
            startTime = Time.time;
            initialApplication = false;
        }
    }
}
