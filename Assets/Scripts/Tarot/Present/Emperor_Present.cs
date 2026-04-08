using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Emperor_Present : Present_TarotCard
{
    float[] fireRatePercent = {1.5f, 1.7f, 1.9f, 2.1f, 2.3f};
    int[] ammoCapaciityIncrease = {20, 30, 40, 50, 60};
    int[] basicDefenseIncrease = {50, 70, 90, 110, 130};
    float knockBackAmount = 0.1f;
    string knockBackDisplay = "0.1";
    EffectManager effectManager = PlayerController.instance.gameObject.GetComponent<EffectManager>();

    // Effects when standing still, and their corresponding effect instance (if any)
    List<Effects> idleEffects = new();
    List<EffectInstance> idleEi = new();

    public Emperor_Present(int q) : base(q)
    {
        cardName = "Emperor_Present";
        arcana = Arcana.Emperor;

        idleEffects.Add(new FireRate_Effect(-1, fireRatePercent[level]));
        idleEffects.Add(new AmmoCapacity_FlatEffect(-1, ammoCapaciityIncrease[level]));
        idleEffects.Add(new Fortified_Additive_Effect(-1, basicDefenseIncrease[level]));

        idleEi = Enumerable.Repeat<EffectInstance>(null, idleEffects.Count).ToList();

        HandlePlayerStateChange(PlayerController.instance.currentState);

        // TODO knockback effect
        // Don't use Knockback_Effect here. It is a debuff that says the entity w/ this effect gets knocked away from damage sources
        // should prob create a new buff effect, and code it so the entity w/ this effect knocks away other entities when dealing dmg
        // effects.Add(new Knockback_Effect(-1));
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
            for (int i = 0; i < idleEffects.Count; ++i)
            {
                idleEi[i] = effectManager.AddBuff(idleEffects[i]);
            }
        }
        else
        {
            for (int i = 0; i < idleEffects.Count; ++i)
            {
                if (idleEi[i] != null)
                {
                    effectManager.RemoveEffect(idleEi[i]);
                    idleEi[i] = null;
                }
            }
        }
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] {
            FormatPlusOnePercentage(fireRatePercent[level]),
            ammoCapaciityIncrease[level],
            basicDefenseIncrease[level],
            knockBackDisplay
        };
    }
}
