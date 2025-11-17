using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Present_TarotCard : TarotCard
{
    public List<Effects> effects;

    public Present_TarotCard(int q) : base(q)
    {
        
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        if (effects != null) { 
            foreach (Effects e in effects)
            {
                tarotManager.effectManager.AddEffect(e);
            }
        }
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        if (effects != null) { 
            foreach (Effects e in effects)
            {
                
            }
        }
    }
}
