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

    // only used for buffs that are not Disable-type
    // stores the sum of all effect modifiers for a given stat
    // contains a pair: the stat to modify, and how to modify it (additive, flat, percentage)
    public Dictionary<Tuple<EffectScriptableObject.Stat, EffectScriptableObject.Application>, float> buffValues = new();

    // only used for debuffs that are not Disable-type
    // stores all the debuffs of a given type, sorted in decreasing order of magnitude
    public Dictionary<Tuple<EffectScriptableObject.Stat, EffectScriptableObject.Application>, SortedSet<EffectScriptableObject>> debuffs = new();

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

        if (existingEffect)
        {
            if (effect.isDebuff)
            {
                // debuffs can't stack, no need to increment effectStacks
                debuffs[key].Add(effect);
            }
            else
            {
                effectStacks[stat]++;
                buffValues[key] += effect.effectRate;
            }
        }
        else
        {
            effectStacks[stat] = 1;
            if (effect.isDebuff)
            {
                debuffs[key] = new(new DebuffComparer())
                {
                    effect
                };
            }
            else
            {
                if (app != EffectScriptableObject.Application.Disable)
                {
                    buffValues[key] = effect.effectRate;
                }
            }
        }

        if (!effect.isPermanent)
        {
            effectTimers.Add(eff);
        }
    }

    // Requires that the instance of EffectInstance is the same one that is stored in this EffectManager's collections
    // b/c this method makes use of ReferenceEquals
    public void RemoveEffect(EffectInstance ei)
    {
        effectTimers.Remove(ei);

        EffectScriptableObject effect = ei.effect;

        EffectScriptableObject.Stat stat = effect.effectStat;
        EffectScriptableObject.Application app = effect.effectApplication;
        Tuple<EffectScriptableObject.Stat, EffectScriptableObject.Application> key = Tuple.Create(stat, app);

        if (effect.isDebuff)
        {
            debuffs[key].Remove(effect);

            if (debuffs[key].Count == 0)
            {
                // this effect no longer exists
                effectStacks[stat] = 0;
                debuffs.Remove(key);
            }
        }
        else
        {
            effectStacks[stat]--;

            if (effectStacks[stat] == 0)
            {
                // this effect no longer exists
                effectStacks.Remove(stat);
                if (app != EffectScriptableObject.Application.Disable)
                {
                    buffValues.Remove(key);
                }
            }
            else
            {
                if (buffValues.ContainsKey(key))
                {
                    buffValues[key] -= effect.effectRate;
                }
            }
        }
    }

    void Update()
    {
        // TODO do timer stuff?
        // maybe? increase the value of incremental EffectInstances by checking if eff.isNextTrigger() is true
        // after calling eff.subtractTime(t)
    }

    public float GetTotalBuff(Tuple<EffectScriptableObject.Stat, EffectScriptableObject.Application> effect)
    {
        switch (effect.Item2)
        {
            case EffectScriptableObject.Application.Additive:
            case EffectScriptableObject.Application.Flat:
                return buffValues.ContainsKey(effect) ? buffValues[effect] : 0f;
            case EffectScriptableObject.Application.Multiplier:
                return buffValues.ContainsKey(effect) ? buffValues[effect] : 1f;
            default:
                return 0f;
        }
    }

    public float GetTotalDebuff(Tuple<EffectScriptableObject.Stat, EffectScriptableObject.Application> effect)
    {
        // Assming that debuffs are either negative (flat/additive) or between 0-1 (percentage reduction),
        // we should use Min instead of Max to find the debuff with the largest reduction value
        switch (effect.Item2)
        {
            case EffectScriptableObject.Application.Additive:
            case EffectScriptableObject.Application.Flat:
                return debuffs.ContainsKey(effect) ? debuffs[effect].Min.effectRate : 0f;
            case EffectScriptableObject.Application.Multiplier:
                return debuffs.ContainsKey(effect) ? debuffs[effect].Min.effectRate : 1f;
            default:
                return 0f;
        }
    }

    public float ModifyStat(EffectScriptableObject.Stat stat, float baseValue)
    {
        // TODO: ensure baseStat doesn't go out of legal bounds (maybe Mathf.Clamp it)
        // large Additive-type debuffs could easily tip a positive stat into negative territory

        // additive
        baseValue += GetTotalBuff(Tuple.Create(stat, EffectScriptableObject.Application.Additive));
        baseValue += GetTotalDebuff(Tuple.Create(stat, EffectScriptableObject.Application.Additive));

        // multiplier
        baseValue *= GetTotalBuff(Tuple.Create(stat, EffectScriptableObject.Application.Multiplier));
        baseValue *= GetTotalDebuff(Tuple.Create(stat, EffectScriptableObject.Application.Multiplier));

        // flat
        baseValue += GetTotalBuff(Tuple.Create(stat, EffectScriptableObject.Application.Flat));
        baseValue += GetTotalDebuff(Tuple.Create(stat, EffectScriptableObject.Application.Flat));

        return baseValue;
    }
}