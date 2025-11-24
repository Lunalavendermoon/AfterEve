using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Present_TarotCard : TarotCard
{
    public List<Effects> effects;
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
