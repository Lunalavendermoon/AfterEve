using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Present_TarotCard : TarotCard
{
    public List<Effects> effects;

    public override void ApplyCard(TarotManager tarotManager)
    {
        foreach (Effects e in effects)
        {
            tarotManager.effectManager.AddEffect(e);
        }
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        throw new System.NotImplementedException();
    }
}
