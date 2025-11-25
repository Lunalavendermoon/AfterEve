using System;
using UnityEngine;

public class PlayerEffectManager : EffectManager
{
    // constant, storing the base attribute values
    public PlayerAttributes basePlayerAttributes;

    // changes during runtime based on the effects the player currently has
    public PlayerAttributes effectPlayerAttributes;

    public static event Action OnEffectAdded;

    public override EffectInstance AddEffect(Effects effect)
    {
        OnEffectAdded?.Invoke();
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
}