using UnityEngine;

public class EffectInstance
{
    public Effects effect;

    public float timer;

    // nextTriggerTime & triggerInterval are only used for Incremental effects
    float nextTriggerTime;

    readonly float triggerInterval;

    public EffectInstance(Effects eff)
    {
        effect = eff;
        timer = eff.effectDuration;
        nextTriggerTime = eff.effectDuration;

        if (eff.IsIncremental())
        {
            triggerInterval = eff.GetIncrementDuration();
        }
    }

    public void SubtractTime(float delta_t)
    {
        timer -= delta_t;
    }

    public bool IsExpired()
    {
        return !effect.isPermanent && timer <= 0f;
    }
}