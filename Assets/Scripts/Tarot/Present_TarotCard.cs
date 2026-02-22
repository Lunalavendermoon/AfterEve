using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;

public class Present_TarotCard : TarotCard
{
    public List<Effects> effects = new();
    public int level;

    public Present_TarotCard(int q) : base(q)
    {
        level = q-1;
        if(level > 4) level = 4;
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        if (effects != null) { 
            foreach (Effects e in effects)
            {
                tarotManager.effectManager.AddEffect(e, PlayerController.instance.playerAttributes);
            }
        }
        ApplyListeners();
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        if (effects != null) { 
            foreach (Effects e in effects)
            {
                
            }
        }
        RemoveListeners();
    }

    protected virtual void ApplyListeners() {}
    protected virtual void RemoveListeners() {}

    protected override void GetLocalizedDesc()
    {
        desc = new LocalizedString
        {
            TableReference = "PresentTarotTable"
        };
    }
}
