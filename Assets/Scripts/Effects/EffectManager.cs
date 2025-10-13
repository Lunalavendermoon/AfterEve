using System.Collections.Generic;
using UnityEngine;

// Manages the effects present on a single entity (player/enemy)
public class EffectManager : MonoBehaviour
{
    List<EffectInstance> effectTimers = new List<EffectInstance>();

    public Dictionary<EffectScriptableObject.Stat, List<EffectInstance>> effectStacks =
        new Dictionary<EffectScriptableObject.Stat, List<EffectInstance>>();

    public bool hasEffect(EffectScriptableObject.Stat stat)
    {
        return effectStacks.ContainsKey(stat);
    }

    public void addEffect(EffectScriptableObject effect)
    {
        bool existingEffect = hasEffect(effect.effectStat);
        if (effect.effectApplication == EffectScriptableObject.Application.Disable &&
            existingEffect)
        {
            // disable is a special case that can't stack
            return;
        }
        EffectInstance eff = new EffectInstance(effect);
        effectTimers.Add(eff);

        if (existingEffect)
        {
            effectStacks[effect.effectStat].Add(eff);
        } else
        {
            effectStacks[effect.effectStat] = new List<EffectInstance> { eff };
        }
    }
    
    void Update()
    {
        // TODO do timer stuff?
    }
}