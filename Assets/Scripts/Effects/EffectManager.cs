using System;
using System.Collections.Generic;
using UnityEngine;

// Manages the effects present on a single entity (player/enemy)
public class EffectManager : MonoBehaviour
{
    // only store effects that can time out
    List<EffectInstance> effectTimers = new();

    // stores number of stack of each effect
    public Dictionary<EffectScriptableObject.Stat, int> effectStacks = new();

    // will not be used for Disable-type effects
    // stores the sum of all effect modifiers for a given stat
    // contains a pair: the stat to modify, and how to modify it (additive, flat, percentage)
    public Dictionary<Tuple<EffectScriptableObject.Stat, EffectScriptableObject.Application>, float> effectValues = new();

    public bool HasEffect(EffectScriptableObject.Stat stat)
    {
        return effectStacks.ContainsKey(stat);
    }

    public void AddEffect(EffectScriptableObject effect)
    {
        EffectScriptableObject.Stat stat = effect.effectStat;
        EffectScriptableObject.Application app = effect.effectApplication;
        Tuple<EffectScriptableObject.Stat, EffectScriptableObject.Application> key = Tuple.Create(stat, app);

        bool existingEffect = HasEffect(stat);
        if (app == EffectScriptableObject.Application.Disable &&
            existingEffect)
        {
            // disable is a special case that can't stack
            return;
        }
        EffectInstance eff = new EffectInstance(effect);
        effectTimers.Add(eff);

        if (existingEffect)
        {
            effectStacks[stat]++;
            effectValues[key] += effect.effectRate;
        }
        else
        {
            effectStacks[stat] = 1;
            if (app != EffectScriptableObject.Application.Disable)
            {
                effectValues[key] = effect.effectRate;
            }
        }
    }
    
    // Assumes the effect has already been removed from effectTimers!!!
    public void RemoveEffect(EffectScriptableObject effect)
    {
        EffectScriptableObject.Stat stat = effect.effectStat;
        EffectScriptableObject.Application app = effect.effectApplication;
        Tuple<EffectScriptableObject.Stat, EffectScriptableObject.Application> key = Tuple.Create(stat, app);

        effectStacks[stat]--;
        
        if (effectStacks[stat] == 0)
        {
            // this effect no longer exists
            effectStacks.Remove(stat);
            if (app != EffectScriptableObject.Application.Disable)
            {
                effectValues.Remove(key);
            }
        } else
        {
            if (effectValues.ContainsKey(key))
            {
                effectValues[key] -= effect.effectRate;
            }
        }
    }
    
    void Update()
    {
        // TODO do timer stuff?
        // maybe? increase the value of incremental EffectInstances by checking if eff.isNextTrigger() is true
        // after calling eff.subtractTime(t)
    }
}