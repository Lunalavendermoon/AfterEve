using UnityEngine;

public class EffectInstance
{
    public Effects effect;

    public float timer;

    public EffectInstance(Effects eff)
    {
        effect = eff;
        timer = eff.effectDuration;
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