using System;
using UnityEngine;

public class PlayerEffectManager : EffectManager
{
    // constant, storing the base attribute values
    public PlayerAttributes basePlayerAttributes;

    // changes during runtime based on the effects the player currently has
    public PlayerAttributes effectPlayerAttributes;

    public static event Action OnEffectAdded;

    public override void AddEffect(Effects effect)
    {
        OnEffectAdded.Invoke();
        base.AddEffect(effect);
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