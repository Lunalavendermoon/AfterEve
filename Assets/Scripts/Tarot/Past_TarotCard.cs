using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;

public abstract class Past_TarotCard : TarotCard
{
    public List<ShieldInstance> effects = new();

    public Past_TarotCard(int q) : base(q)
    {
        GetLocalizedDesc();
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        // if (effects != null) { 
        //     foreach (Effects e in effects)
        //     {
        //         tarotManager.effectManager.AddEffect(e);
        //     }
        // }
        ApplyListenersEffects();
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        // TODO - can past tarot cards ever be removed/toggled off?

        // if (effects != null) { 
        //     foreach (Effects e in effects)
        //     {
                
        //     }
        // }
        // RemoveListeners();
    }

    protected virtual void ApplyListenersEffects() {}

    protected override void GetLocalizedDesc()
    {
        desc = new LocalizedString
        {
            TableReference = "PastTarotTable"
        };
    }

    protected virtual void SetDescriptionValues()
    {
        // override in each child
    }
}
