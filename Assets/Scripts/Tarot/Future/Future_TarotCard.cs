using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Future_TarotCard : TarotCard
{
    protected Future_Reward reward;

    public Future_TarotCard(string s, int q) : base(s, q)
    {
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        // TODO
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        // TODO
    }

    public virtual void RewardPlayer()
    {
        // TODO give reward to player
    }
}
