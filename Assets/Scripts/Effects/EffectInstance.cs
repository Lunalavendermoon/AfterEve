using UnityEngine;
using UnityEngine.VFX;

public class EffectInstance
{
    public Effects effect;

    public float timer;

    // nextTriggerTime & triggerInterval are only used for Incremental effects
    float nextTriggerTime;

    readonly float triggerInterval;

    bool initialApplication = true;

    // used to compare unique debuff instances in the case that their effectRates are equal
    public long effectId;

    public EffectInstance(Effects eff, long effectId)
    {
        effect = eff;
        timer = eff.effectDuration;
        nextTriggerTime = eff.effectDuration;

        this.effectId = effectId;

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
        return timer <= nextTriggerTime || initialApplication;
    }

    public void DecrementTriggerTime()
    {
        if (IsNextTrigger())
        {
            nextTriggerTime -= triggerInterval;
            initialApplication = false;
        }
    }

    public bool IsTriggered()
    {
        return !effect.IsIncremental() || IsNextTrigger();
    }
}