using System.Collections.Generic;
using UnityEngine;

public class Emperor_Present : Present_TarotCard
{
    float[] fireRatePercent = {1.5f, 1.7f, 1.9f, 2.1f, 2.3f};
    int[] ammoCapaciityIncrease = {20, 30, 40, 50, 60};
    int[] basicDefenseIncrease = {50, 70, 90, 110, 130};
    float knockBackAmount = 0.1f;
    EffectManager effectManager = PlayerController.instance.gameObject.GetComponent<EffectManager>();

    public Emperor_Present(int q) : base(q)
    {
        cardName = "Emperor_Present";

        effects.Add(new FireRate_Effect(-1, fireRatePercent[level]));
        effects.Add(new AmmoCapacity_Effect(-1, ammoCapaciityIncrease[level]));
        effects.Add(new Fortified_Additive_Effect(-1, basicDefenseIncrease[level]));
        effects.Add(new Knockback_Effect(-1)); // TODO set Knockback a specific amount?
    }

    protected override void ApplyListeners()
    {
        PlayerController.OnPlayerStateChange += HandlePlayerStateChange;
    }

    protected override void RemoveListeners()
    {
        PlayerController.OnPlayerStateChange -= HandlePlayerStateChange;
    }

    private void HandlePlayerStateChange(IPlayerState state)
    {
        if(state is Player_Idle) 
        {
            foreach(Effects e in effects)
            {
                effectManager.AddEffect(e);
            }
        }
        else
        {
            foreach(Effects e in effects)
            {
                //effectManager.RemoveEffect(e);
            }
        }
    }
}
