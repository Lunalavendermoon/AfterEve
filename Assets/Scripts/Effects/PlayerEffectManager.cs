using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class PlayerEffectManager : EffectManager
{
    // constant, storing the base attribute values
    public PlayerAttributes basePlayerAttributes;

    // changes during runtime based on the effects the player currently has
    public PlayerAttributes effectPlayerAttributes;

    public GameObject genericEffectIcon;

    public EffectIcons effectIcons;

    public GameObject effectIconDisplay;

    // hard cap on max number of effect icons
    public int effectIconCap = 5;
    int effectIconCount = 0;

    public static event Action OnEffectAdded;

    Dictionary<Effects.IconType, int> typeCounts = new();

    Dictionary<Effects.IconType, GameObject> iconInstances = new();

    // List<Effects.IconType> iconQueue = new();

    public override EffectInstance AddBuff(Effects effect)
    {
        return AddEffect(effect, null);
    }
        
    public override EffectInstance AddEffect(Effects effect, EntityAttributes attr)
    {
        OnEffectAdded?.Invoke();

        if (effect.iconType != Effects.IconType.None)
        {
            AddIcon(effect.iconType);
            // if (iconCounts.TryAdd(effect.iconType, 1))
            // {
            //     Debug.LogWarning(effectIconDisplay == null);
            //     AddIcon(effect.iconType);
            // }
            // else
            // {
            //     if (iconCounts[effect.iconType] == 0)
            //     {
            //         ++iconCounts[effect.iconType];
            //         AddIcon(effect.iconType);
            //     }
            // }
        }

        if (!effect.isDebuff && effect.hasVfx)
        {
            //TODO: no vfx for corrode, hitcountshield, and pierce
            switch (effect.effectStat)
            {
                //Speed
                //TODO: is haste the same as speed?
                case Effects.Stat.Haste:
                    vfx.EnableSpeed();
                    break;
                //Regen
                case Effects.Stat.HP:
                    vfx.EnableRegen();
                    break;
                //Strength
                case Effects.Stat.Damage:
                    vfx.EnableStrength();
                    break;
                //Fortify
                case Effects.Stat.BasicDefense:
                    vfx.EnableFortify();
                    break;
                //Bless
                case Effects.Stat.SpiritualDefense:
                    vfx.EnableBless();
                    break;
                //Enlighten
                case Effects.Stat.SpiritualVision:
                    vfx.SetEnlightenTime(effect.effectDuration);
                    vfx.EnableEnlighten();
                    break;
                //Luck
                case Effects.Stat.Luck:
                    vfx.EnableLuck();
                    break;
            }
        }
        
        return base.AddEffect(effect, attr);
    }

    public override void RemoveEffect(EffectInstance ei, bool muted = false)
    {
        Effects effect = ei.effect;

        if (effect.iconType != Effects.IconType.None)
        {
            --typeCounts[effect.iconType];

            if (typeCounts[effect.iconType] == 0)
            {
                RemoveIcon(effect.iconType);
            }
        }


        if (!effect.isDebuff && effect.hasVfx)
        {
            switch (effect.effectStat)
            {
                //Speed
                case Effects.Stat.Haste:
                    vfx.DisableSpeed();
                    break;
                //Regen
                case Effects.Stat.HP:
                    vfx.DisableRegen();
                    break;
                //Strength
                case Effects.Stat.Damage:
                    vfx.DisableStrength();
                    break;
                //Fortify
                case Effects.Stat.BasicDefense:
                    vfx.DisableFortify();
                    break;
                //Bless
                case Effects.Stat.SpiritualDefense:
                    vfx.DisableBless();
                    break;
                //Enlighten
                case Effects.Stat.SpiritualVision:
                    vfx.DisableEnlighten();
                    break;
                //Luck
                case Effects.Stat.Luck:
                    vfx.DisableLuck();
                    break;
            }
        }

        base.RemoveEffect(ei, muted);
    }

    public override void ApplyEffects()
    {
        effectPlayerAttributes = Instantiate(basePlayerAttributes);
        ApplyEffectsHelper((effect, increment) => { effect.ApplyPlayerEffect(effectPlayerAttributes, increment); });
        PlayerController.instance.playerAttributes = effectPlayerAttributes;

        // just for testing, comment this out if needed
        // Debug.Log("Current Basic Defense: " + effectPlayerAttributes.basicDefense);
    }

    public void AddIcon(Effects.IconType type)
    {
        if (typeCounts.ContainsKey(type) && typeCounts[type] >= 1)
        {
            return;
        }

        Debug.Log($"Display effect {type} - {GetInstanceID()}");

        GameObject go = Instantiate(genericEffectIcon, effectIconDisplay.transform);
        go.GetComponent<Image>().sprite = getIconSprite(type);
        
        if (!typeCounts.TryAdd(type, 1))
        {
            ++typeCounts[type];
        }
        if (!iconInstances.TryAdd(type, go))
        {
            iconInstances[type] = go;
        }

        // if (typeCounts.ContainsKey(type) && typeCounts[type] >= 1)
        // {
        //     return;
        // }

        // ++effectIconCount;
        
        // if (!typeCounts.TryAdd(type, 1))
        // {
        //     ++typeCounts[type];
        // }

        // if (effectIconCount >= effectIconCap)
        // {
        //     // add to queue
        //     iconQueue.Add(type);
        // }
        // else
        // {
        //     GameObject go = Instantiate(genericEffectIcon, effectIconDisplay.transform);
        //     go.GetComponent<Image>().sprite = getIconSprite(type);
        //     if (!iconInstances.TryAdd(type, go))
        //     {
        //         iconInstances[type] = go;
        //     }
        // }
    }

    public void RemoveIcon(Effects.IconType type)
    {
        --effectIconCount;
        Destroy(iconInstances[type]);

        // bool wasRemoved = iconQueue.Remove(type);

        // if (!wasRemoved)
        // {
        //     // pop queue
        //     Effects.IconType qtype = iconQueue[iconQueue.Count - 1];
        //     iconQueue.RemoveAt(iconQueue.Count - 1);

        //     ++effectIconCount;

        //     GameObject go = Instantiate(genericEffectIcon, effectIconDisplay.transform);
        //     go.GetComponent<Image>().sprite = getIconSprite(qtype);
        //     if (!iconInstances.TryAdd(qtype, go))
        //     {
        //         iconInstances[qtype] = go;
        //     }
        // }
    }

    Sprite getIconSprite(Effects.IconType type)
    {
        return type switch
        {
            Effects.IconType.BuffBless => effectIcons.bless,
            Effects.IconType.BuffEnlighten => effectIcons.enlighten,
            Effects.IconType.BuffFortify => effectIcons.fortify,
            Effects.IconType.BuffLucky => effectIcons.lucky,
            Effects.IconType.BuffRegen => effectIcons.regen,
            Effects.IconType.BuffShield => effectIcons.shield,
            Effects.IconType.BuffSpeed => effectIcons.speed,
            Effects.IconType.BuffStrength => effectIcons.strength,
            Effects.IconType.DebuffBleed => effectIcons.bleed,
            Effects.IconType.DebuffBlind => effectIcons.blind,
            Effects.IconType.DebuffBurn => effectIcons.burn,
            Effects.IconType.DebuffConfused => effectIcons.confused,
            Effects.IconType.DebuffKnockback => effectIcons.knockback,
            Effects.IconType.DebuffParalyze => effectIcons.paralyze,
            Effects.IconType.DebuffSlow => effectIcons.slow,
            Effects.IconType.DebuffSundered => effectIcons.sundered,
            Effects.IconType.DebuffWeak => effectIcons.weak,
            _ => null,
        };
    }
}