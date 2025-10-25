using UnityEngine;

public class EffectInstance
{
    public Effects effect;

    float timer;

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

    // might be a bit dodgy implementation...
    // currently, relies on the effect manager to remember to call IsNextTrigger() every frame
    // to check if Incremental effect has triggered and to make this class
    // set the timestamp for the next trigger
    public bool IsNextTrigger()
    {
        if (!effect.IsIncremental())
        {
            return false;
        }
        return timer <= nextTriggerTime;
    }

    public void DecrementTriggerTime()
    {
        if (!effect.IsIncremental())
        {
            return;
        }
        nextTriggerTime -= triggerInterval;
    }
}