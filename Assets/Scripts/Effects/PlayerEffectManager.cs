using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEffectManager : EffectManager
{
    //visual vfx manager
    [SerializeField] private PlayerVFXManager vfx;

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

    // TODO: remove deubg code
    //------------------------------- DEBUG - START -------------------------------------
    public enum Effect
    {
        speed,
        regen,
        strength,
        fortify,
        bless,
        enlighten,
        luck,
        none
    };
    public Effect currentState = Effect.none;
    private EffectInstance regen;
    private EffectInstance strength;
    private EffectInstance fortify;
    private EffectInstance bless;
    private EffectInstance enlighten;
    private EffectInstance luck;
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            switch (currentState)
            {
                case Effect.speed:
                    break;
                case Effect.regen:
                    regen = AddEffect(new Regeneration_Effect(5f, 2f));
                    break;
                case Effect.strength:
                    strength = AddEffect(new Strength_Effect(5f, 2f));
                    break;
                case Effect.fortify:
                    fortify = AddEffect(new Fortified_Effect(5f, 2f));
                    break;
                case Effect.bless:
                    bless = AddEffect(new Blessed_Effect(5f, 2f));
                    break;
                case Effect.enlighten:
                    enlighten = AddEffect(new Enlightened_Effect(5f));
                    break;
                case Effect.luck:
                    luck = AddEffect(new Lucky_Effect(5f, 5));
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            switch (currentState)
            {
                case Effect.speed:
                    break;
                case Effect.regen:
                    if (regen != null) RemoveEffect(regen);
                    regen = null;
                    break;
                case Effect.strength:
                    if (strength != null) RemoveEffect(strength);
                    strength = null;
                    break;
                case Effect.fortify:
                    if (fortify != null) RemoveEffect(fortify);
                    fortify = null;
                    break;
                case Effect.bless:
                    if (bless != null) RemoveEffect(bless);
                    bless = null;
                    break;
                case Effect.enlighten:
                    if (enlighten != null) RemoveEffect(enlighten);
                    enlighten = null;
                    break;
                case Effect.luck:
                    if (luck != null) RemoveEffect(luck);
                    luck = null;
                    break;
            }
        }
    }
    //------------------------------- DEBUG - END -------------------------------------
        
    public override EffectInstance AddEffect(Effects effect)
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
            //TODO: no vfx for corrode, haste, hitcountshield, and pierce
            switch (effect.effectStat)
            {
                //Speed
                //TODO: currently no speed effect class
                case Effects.Stat.Speed:
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
                //TODO: currently assumes countdown is 5 seconds, need to make adaptive
                case Effects.Stat.SpiritualVision:
                    vfx.EnableEnlighten();
                    break;
                //Luck
                case Effects.Stat.Luck:
                    vfx.EnableLuck();
                    break;
            }
        }
        

        return base.AddEffect(effect);
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
                case Effects.Stat.Speed:
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
                    //Enlighten countdown handles itself
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