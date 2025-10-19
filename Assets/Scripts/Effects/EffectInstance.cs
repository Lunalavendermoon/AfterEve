using UnityEngine;

public class EffectInstance
{
    public Effects effect;

    float timer;

    // nextTriggerTime & triggerInterval are only used for Incremental effects
    float nextTriggerTime;

    float triggerInterval;

    public EffectInstance(Effects eff)
    {
        effect = eff;
        timer = eff.effectDuration;
        nextTriggerTime = eff.effectDuration;

        if (eff.isIncremental)
        {
            triggerInterval = eff.incrementInterval;
        }
    }

    public void subtractTime(float delta_t)
    {
        timer -= delta_t;
    }

    public bool isExpired()
    {
        return !effect.isPermanent && timer <= 0f;
    }

    // might be a bit dodgy implementation...
    // currently, relies on the effect manager to remember to call isNextTrigger() every frame
    // to check if Incremental effect has triggered and to make this class
    // set the timestamp for the next trigger
    public bool isNextTrigger()
    {
        if (!effect.isIncremental)
        {
            return false;
        }

        bool ret = false;
        if (timer <= nextTriggerTime)
        {
            ret = true;
            nextTriggerTime -= triggerInterval;
        }
        return ret;
    }
}