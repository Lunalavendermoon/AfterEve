using System;
using System.Collections.Generic;
using UnityEngine;


// Manages the effects present on a single entity (player/enemy)
public class EffectManager : MonoBehaviour
{
    // only store effects that can time out
    readonly List<EffectInstance> effectTimers = new();

    // only store effects that never time out (like tarot effects)
    readonly List<EffectInstance> permanentEffects = new();

    // stores every single effect that's currently active
    public Dictionary<Tuple<Effects.Stat, Effects.Application>, List<Effects>> effectStacks = new();

    // only used for buffs that are not Disable-type
    // stores the all buff modifiers for a given stat
    // contains a pair: the stat to modify, and how to modify it (additive, flat, percentage)
    public Dictionary<Tuple<Effects.Stat, Effects.Application>, List<Effects>> buffs = new();

    // only used for debuffs that are not Disable-type
    // stores all the debuffs of a given type, sorted in decreasing order of magnitude
    public Dictionary<Tuple<Effects.Stat, Effects.Application>, SortedSet<Effects>> debuffs = new();

    public void AddEffect(Effects effect)
    {
        Effects.Stat stat = effect.effectStat;
        Effects.Application app = effect.effectApplication;
        Tuple<Effects.Stat, Effects.Application> key = Tuple.Create(stat, app);

        bool existingEffect = effectStacks.ContainsKey(key);

        if (app == Effects.Application.Disable &&
            existingEffect && !effect.isPermanent)
        {
            // disable is a special case that can't stack
            // instead, if the current debuff has a longer duration than the currently active debuff,
            // increase the duration of the active debuff to simulate the act of applying a new disable debuff
            foreach (EffectInstance ei in effectTimers)
            {
                if (ei.effect.effectStat == stat)
                {
                    ei.timer = Mathf.Max(ei.timer, effect.effectDuration);
                    return;
                }
            }
        }
        
        EffectInstance eff = new EffectInstance(effect);

        if (existingEffect)
        {
            if (effect.isDebuff)
            {
                // debuffs can't stack, no need to increment effectStacks
                debuffs[key].Add(effect);
            }
            else
            {
                effectStacks[key].Add(effect);
                buffs[key].Add(effect);
            }
        }
        else
        {
            effectStacks[key] = new() { effect };
            if (effect.isDebuff)
            {
                debuffs[key] = new(new DebuffComparer())
                {
                    effect
                };
            }
            else
            {
                if (app != Effects.Application.Disable)
                {
                    buffs[key] = new List<Effects>() { effect };
                }
            }
        }

        if (effect.isPermanent)
        {
            permanentEffects.Add(eff);
        } else
        {
            effectTimers.Add(eff);
        }

        ApplyEffects();
    }

    // Requires that the instance of EffectInstance is the same one that is stored in this EffectManager's collections
    // b/c this method makes use of ReferenceEquals
    public void RemoveEffect(EffectInstance ei)
    {
        if (ei.effect.isPermanent)
        {
            permanentEffects.Remove(ei);
        } else
        {
            effectTimers.Remove(ei);
        }

        Effects effect = ei.effect;

        Effects.Stat stat = effect.effectStat;
        Effects.Application app = effect.effectApplication;
        Tuple<Effects.Stat, Effects.Application> key = Tuple.Create(stat, app);

        if (effect.isDebuff)
        {
            debuffs[key].Remove(effect);

            if (debuffs[key].Count == 0)
            {
                // this effect no longer exists
                effectStacks.Remove(key);
                debuffs.Remove(key);
            }
        }
        else
        {
            effectStacks[key].Remove(effect);

            if (effectStacks[key].Count == 0)
            {
                // this effect no longer exists
                effectStacks.Remove(key);
                if (app != Effects.Application.Disable)
                {
                    buffs.Remove(key);
                }
            }
            else
            {
                if (buffs.ContainsKey(key))
                {
                    buffs[key].Remove(effect);
                }
            }
        }
    }

    void Update()
    {
        float time_elapsed = Time.deltaTime;
        bool reapply_effects = false;

        for(int i =effectTimers.Count - 1; i >= 0; i--)
        {
            EffectInstance ei = effectTimers[i];
            ei.SubtractTime(time_elapsed);
            if (ei.IsExpired())
            {
                RemoveEffect(ei);
                reapply_effects = true;
            }
            else if (ei.IsNextTrigger())
            {
                reapply_effects = true;
            }
        }

        if (reapply_effects)
        {
            ApplyEffects();
        }
    }

    public void ApplyEffects()
    {
        // TODO we need to reset player stats to base before applying
        // not sure if Effect already does that, if it does then ignore this comment :D

        // iterate through all stats and modify them one-by-one!
        // this is the easiest way but kind of inefficient
        // because some combinations of stat and application will never exist (ex. Defense and Disable)
        // if this impacts performance, i'll fix it to skip over the impossible cases
        Effects.Stat[] stats = (Effects.Stat[])Enum.GetValues(typeof(Effects.Stat));
        Effects.Application[] applications = (Effects.Application[])Enum.GetValues(typeof(Effects.Application));

        List<Effects.Application> ordered = new() {
            Effects.Application.Additive, Effects.Application.Multiplier, Effects.Application.Flat };

        // apply all additive, multiplier, and flat effects
        foreach (Effects.Stat stat in stats)
        {
            foreach (Effects.Application app in ordered)
            {
                Tuple<Effects.Stat, Effects.Application> key = Tuple.Create(stat, app);

                List<Effects> buffList;
                if (buffs.TryGetValue(key, out buffList))
                {
                    foreach (Effects eff in buffList)
                    {
                        eff.ApplyEffect();
                    }
                }

                SortedSet<Effects> debuffSet;
                if (debuffs.TryGetValue(key, out debuffSet))
                {
                    debuffSet.Min.ApplyEffect();
                }
            }
        }

        // apply all other effects
        foreach (Effects.Stat stat in stats)
        {
            foreach (Effects.Application app in applications)
            {
                if (ordered.Contains(app))
                {
                    // already applied these in the previous loop!
                    continue;
                }

                Tuple<Effects.Stat, Effects.Application> key = Tuple.Create(stat, app);

                if (effectStacks.ContainsKey(key))
                {
                    foreach (Effects eff in effectStacks[key])
                    {
                        eff.ApplyEffect();
                    }
                }
            }
        }
    }
}