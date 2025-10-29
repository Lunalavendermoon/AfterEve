using UnityEngine;

public class PlayerEffectManager : EffectManager
{
    // constant, storing the base attribute values
    public PlayerAttributes basePlayerAttributes;

    // changes during runtime based on the effects the player currently has
    public PlayerAttributes effectPlayerAttributes;

    public override void ApplyEffects()
    {
        effectPlayerAttributes = Instantiate(basePlayerAttributes);
        ApplyEffectsHelper((effect) => { effect.ApplyPlayerEffect(effectPlayerAttributes); });
        PlayerController.instance.playerAttributes = effectPlayerAttributes;

        // just for testing, comment this out if needed
        // Debug.Log("Current Basic Defense: " + effectPlayerAttributes.basicDefense);
    }
}