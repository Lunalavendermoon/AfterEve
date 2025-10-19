using System;
using System.Collections.Generic;
using UnityEngine;


// Manages the effects present on a single entity (player/enemy)
public class EffectManager : MonoBehaviour
{
    // only store effects that can time out
    List<EffectInstance> effectTimers = new();

    // stores number of stack of each effect
    public Dictionary<Effects.Stat, int> effectStacks = new();

    // only used for buffs that are not Disable-type
    // stores the sum of all effect modifiers for a given stat
    // contains a pair: the stat to modify, and how to modify it (additive, flat, percentage)
    public Dictionary<Tuple<Effects.Stat, Effects.Application>, float> buffValues = new();

    // only used for debuffs that are not Disable-type
    // stores all the debuffs of a given type, sorted in decreasing order of magnitude
    public Dictionary<Tuple<Effects.Stat, Effects.Application>, SortedSet<Effects>> debuffs = new();

    public bool HasEffect(Effects.Stat stat)
    {
        return effectStacks.ContainsKey(stat);
    }

    public void AddEffect(Effects effect)
    {
        Effects.Stat stat = effect.effectStat;
        Effects.Application app = effect.effectApplication;
        Tuple<Effects.Stat, Effects.Application> key = Tuple.Create(stat, app);

        bool existingEffect = HasEffect(stat);
        if (app == Effects.Application.Disable &&
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
                if (app != Effects.Application.Disable)
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
                if (app != Effects.Application.Disable)
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

        float time_elapsed=Time.deltaTime;

        for(int i =effectTimers.Count - 1; i >= 0; i--)
        {
            EffectInstance ei = effectTimers[i];
            ei.subtractTime(time_elapsed);
            if (ei.isExpired())
            {
                RemoveEffect(ei);
            }
            else if (ei.isNextTrigger())
            {
                //TODO apply incremental effect
                
            }
        }


    }

    public float GetTotalBuff(Tuple<Effects.Stat, Effects.Application> effect)
    {
        switch (effect.Item2)
        {
            case Effects.Application.Additive:
            case Effects.Application.Flat:
                return buffValues.ContainsKey(effect) ? buffValues[effect] : 0f;
            case Effects.Application.Multiplier:
                return buffValues.ContainsKey(effect) ? buffValues[effect] : 1f;
            default:
                return 0f;
        }
    }

    public float GetTotalDebuff(Tuple<Effects.Stat, Effects.Application> effect)
    {
        // Assming that debuffs are either negative (flat/additive) or between 0-1 (percentage reduction),
        // we should use Min instead of Max to find the debuff with the largest reduction value
        switch (effect.Item2)
        {
            case Effects.Application.Additive:
            case Effects.Application.Flat:
                return debuffs.ContainsKey(effect) ? debuffs[effect].Min.effectRate : 0f;
            case Effects.Application.Multiplier:
                return debuffs.ContainsKey(effect) ? debuffs[effect].Min.effectRate : 1f;
            default:
                return 0f;
        }
    }

    public float ModifyStat(Effects.Stat stat, float baseValue)
    {
        // TODO: ensure baseStat doesn't go out of legal bounds (maybe Mathf.Clamp it)
        // large Additive-type debuffs could easily tip a positive stat into negative territory

        // additive
        baseValue += GetTotalBuff(Tuple.Create(stat, Effects.Application.Additive));
        baseValue += GetTotalDebuff(Tuple.Create(stat, Effects.Application.Additive));

        // multiplier
        baseValue *= GetTotalBuff(Tuple.Create(stat, Effects.Application.Multiplier));
        baseValue *= GetTotalDebuff(Tuple.Create(stat, Effects.Application.Multiplier));

        // flat
        baseValue += GetTotalBuff(Tuple.Create(stat, Effects.Application.Flat));
        baseValue += GetTotalDebuff(Tuple.Create(stat, Effects.Application.Flat));

        return baseValue;
    }
}