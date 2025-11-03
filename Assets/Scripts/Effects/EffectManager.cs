using System;
using System.Collections.Generic;
using UnityEngine;


// Manages the effects present on a single entity (player/enemy)
// Don't use this directly! Instead, use the children classes PlayerEffectManager or EnemyEffectManager :)
public abstract class EffectManager : MonoBehaviour
{
    // only store effects that can time out
    readonly List<EffectInstance> effectTimers = new();

    // only store effects that never time out (like tarot effects)
    readonly List<EffectInstance> permanentEffects = new();

    // stores every single effect that's currently active
    public Dictionary<Tuple<Effects.Stat, Effects.Application>, List<EffectInstance>> effectStacks = new();

    // only used for buffs that are not Disable-type
    // stores the all buff modifiers for a given stat
    // contains a pair: the stat to modify, and how to modify it (additive, flat, percentage)
    public Dictionary<Tuple<Effects.Stat, Effects.Application>, List<EffectInstance>> buffs = new();

    // only used for debuffs that are not Disable-type
    // stores all the debuffs of a given type, sorted in decreasing order of magnitude
    public Dictionary<Tuple<Effects.Stat, Effects.Application>, SortedSet<EffectInstance>> debuffs = new();

    // used to assign effects to effectinstances
    // make it a long instead of int, just in case we somehow have a bajillion effects (probably impossible tbh)
    long effectCount = 0;

    // ONLY FOR TESTING PURPOSES TO ADD A DUMMY EFFECT!!
    public void AddEffectTest(string effect)
    {
        switch (effect)
        {
            case "paralyze":
                Debug.Log("Apply paralyze effect");
                AddEffect(new Paralyze_Effect(5f));
                break;
            case "def flat":
                Debug.Log("Apply Defense Flat effect");
                AddEffect(new Fortified_Flat_Effect(5f, 5));
                break;
            case "def mult":
                Debug.Log("Apply Defense Multiplier effect");
                AddEffect(new Fortified_Effect(5f, 1.5f));
                break;
            case "def add":
                Debug.Log("Apply Defense Additive effect");
                AddEffect(new Fortified_Additive_Effect(5f, 5));
                break;
            case "regen":
                Debug.Log("Apply Regeneration effect");
                AddEffect(new Regeneration_Effect(5f, 0.2f));
                break;
            case "bleed":
                Debug.Log("Apply Bleed effect");
                AddEffect(new Bleed_Effect(5f, 0.1f, 1f));
                break;
            case "sunder big":
                Debug.Log("Apply Sundered debuff (large magnitude)");
                AddEffect(new Sundered_Effect(5f, 0.5f));
                break;
            case "sunder small":
                Debug.Log("Apply Sundered debuff (small magnitude)");
                AddEffect(new Sundered_Effect(5f, 0.8f));
                break;
            case "confused":
                Debug.Log("Apply Confused effect");
                AddEffect(new Confused_Effect(5f));
                break;
            case "slow":
                Debug.Log("Apply Slow effect");
                AddEffect(new Slow_Effect(5f, 0.8f));
                break;
            default:
                Debug.Log(effect + " is not a valid effect");
                break;
        }
    }

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

        EffectInstance eff = new EffectInstance(effect, effectCount++);

        if (existingEffect)
        {
            if (effect.isDebuff)
            {
                // debuffs can't stack, no need to increment effectStacks
                debuffs[key].Add(eff);
            }
            else
            {
                effectStacks[key].Add(eff);
                buffs[key].Add(eff);
            }
        }
        else
        {
            effectStacks[key] = new() { eff };
            if (effect.isDebuff)
            {
                debuffs[key] = new(new DebuffComparer())
                {
                    eff
                };
            }
            else
            {
                if (app != Effects.Application.Disable)
                {
                    buffs[key] = new() { eff };
                }
            }
        }

        if (effect.isPermanent)
        {
            permanentEffects.Add(eff);
        }
        else
        {
            effectTimers.Add(eff);
        }
    }

    // Requires that the instance of EffectInstance is the same one that is stored in this EffectManager's collections
    // b/c this method makes use of ReferenceEquals
    public void RemoveEffect(EffectInstance ei)
    {
        if (ei.effect.isPermanent)
        {
            permanentEffects.Remove(ei);
        }
        else
        {
            effectTimers.Remove(ei);
        }

        Effects effect = ei.effect;

        Effects.Stat stat = effect.effectStat;
        Effects.Application app = effect.effectApplication;
        Tuple<Effects.Stat, Effects.Application> key = Tuple.Create(stat, app);

        if (effect.isDebuff)
        {
            debuffs[key].Remove(ei);

            if (debuffs[key].Count == 0)
            {
                // this effect no longer exists
                effectStacks.Remove(key);
                debuffs.Remove(key);
            }
        }
        else
        {
            effectStacks[key].Remove(ei);

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
                    buffs[key].Remove(ei);
                }
            }
        }
    }

    void Update()
    {
        float time_elapsed = Time.deltaTime;

        // bool makeUpdate = false;

        for (int i = effectTimers.Count - 1; i >= 0; i--)
        {
            EffectInstance ei = effectTimers[i];
            ei.SubtractTime(time_elapsed);
            if (ei.IsExpired())
            {
                RemoveEffect(ei);
            }
            // else if (ei.IsNextTrigger())
            // {
            //     makeUpdate = true;
            // }
        }

        ApplyEffects();
    }

    public abstract void ApplyEffects();

    public void ApplyEffectsHelper(Action<Effects> applyEffect)
    {
        // iterate through all stats and modify them one-by-one!
        // this is the easiest way but kind of inefficient
        // because some combinations of stat and application will never exist (ex. Defense and Disable)
        // if this impacts performance, i'll fix it to skip over the impossible cases
        Effects.Stat[] stats = (Effects.Stat[])Enum.GetValues(typeof(Effects.Stat));
        Effects.Application[] applications = (Effects.Application[])Enum.GetValues(typeof(Effects.Application));

        foreach (Effects.Stat stat in stats)
        {
            foreach (Effects.Application app in applications)
            {
                Tuple<Effects.Stat, Effects.Application> key = Tuple.Create(stat, app);

                if (app == Effects.Application.Additive || app == Effects.Application.Multiplier ||
                    app == Effects.Application.Flat)
                {
                    if (buffs.TryGetValue(key, out List<EffectInstance> buffList))
                    {
                        foreach (EffectInstance eff in buffList)
                        {
                            applyEffect(eff.effect);
                        }
                    }

                    if (debuffs.TryGetValue(key, out SortedSet<EffectInstance> debuffSet))
                    {
                        applyEffect(debuffSet.Min.effect);
                    }
                }
                else
                {
                    if (effectStacks.TryGetValue(key, out List<EffectInstance> effects))
                    {
                        foreach (EffectInstance eff in effects)
                        {
                            applyEffect(eff.effect);
                        }
                    }
                }
            }
        }
    }
}