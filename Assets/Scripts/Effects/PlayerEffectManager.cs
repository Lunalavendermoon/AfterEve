using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEffectManager : EffectManager
{

    // constant, storing the base attribute values
    public PlayerAttributes basePlayerAttributes;

    // changes during runtime based on the effects the player currently has
    public PlayerAttributes effectPlayerAttributes;

    public GameObject genericEffectIcon;

    public EffectIcons effectIcons;

    public GameObject effectIconDisplay;

    public static event Action OnEffectAdded;

    Dictionary<Effects.IconType, int> iconCounts = new();

    Dictionary<Effects.IconType, GameObject> iconInstances = new();

    public override EffectInstance AddBuff(Effects effect)
    {
        return AddEffect(effect, null);
    }
        
    public override EffectInstance AddEffect(Effects effect, EntityAttributes attr)
    {
        OnEffectAdded?.Invoke();

        if (effect.iconType != Effects.IconType.None)
        {
            if (iconCounts.TryAdd(effect.iconType, 1))
            {
                Debug.LogWarning(effectIconDisplay == null);
                AddIcon(effect.iconType);
            }
            else
            {
                if (iconCounts[effect.iconType] == 0)
                {
                    AddIcon(effect.iconType);
                }
                ++iconCounts[effect.iconType];
            }
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

    public override void RemoveEffect(EffectInstance ei)
    {
        if (ei.effect.effectStat == Effects.Stat.HitCountShield)
        {
            // TODO - currently assumes only Hierophant Future applies a hit-count shield, which may not be true
            // This code always attempts to subtract the initial shield amount,
            // but if there are shields from multiple sources (other than Hierophant which only stacks once)
            // then they might interfere w/ each other
            PlayerController.instance.playerAttributes.hitCountShield = Math.Max(
                0, PlayerController.instance.playerAttributes.hitCountShield - ((HitCountShield_Effect)ei.effect).amount);
        }

        Effects effect = ei.effect;

        if (effect.iconType != Effects.IconType.None)
        {
            --iconCounts[effect.iconType];

            if (iconCounts[effect.iconType] == 0)
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

        base.RemoveEffect(ei);
    }

    public override void ApplyEffects()
    {
        effectPlayerAttributes = Instantiate(basePlayerAttributes);
        ApplyEffectsHelper((effect, increment) => { effect.ApplyPlayerEffect(effectPlayerAttributes, increment); });
        PlayerController.instance.playerAttributes = effectPlayerAttributes;

        // just for testing, comment this out if needed
        // Debug.Log("Current Basic Defense: " + effectPlayerAttributes.basicDefense);
    }

    void AddIcon(Effects.IconType type)
    {
        GameObject go = Instantiate(genericEffectIcon, effectIconDisplay.transform);
        go.GetComponent<Image>().sprite = getIconSprite(type);
        
        if (!iconInstances.TryAdd(type, go))
        {
            iconInstances[type] = go;
        }
    }

    void RemoveIcon(Effects.IconType type)
    {
        Destroy(iconInstances[type]);
    }

    Sprite getIconSprite(Effects.IconType type)
    {
        switch (type)
        {
            case Effects.IconType.BuffDefense:
                return effectIcons.defenseBuff;
            case Effects.IconType.BuffSpeed:
                return effectIcons.speedBuff;
            case Effects.IconType.BuffRegen:
                return effectIcons.regeneration;
            case Effects.IconType.BuffStrength:
                return effectIcons.strength;
        }
        return null;
    }
}