using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;

public abstract class Past_TarotCard : TarotCard
{
    public List<Effects> effects = new();
    public List<EffectInstance> effectInstances = new();

    public Past_TarotCard() : base(1)
    {
        tarotType = TarotType.Past;
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        ApplyCard(tarotManager, false);
    }

    public virtual void ApplyCard(TarotManager tarotManager, bool muted)
    {
        foreach (Effects e in effects)
        {
            effectInstances.Add(
                tarotManager.effectManager.AddEffect(e, PlayerController.instance.playerAttributes, true));
        }
        if (effects.Count != 0 && !muted)
        {
            tarotManager.effectManager.ApplyEffects(); // Recalculate attributes
        }
        ApplyListenersEffects(muted);
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        RemoveCard(tarotManager, false);
    }

    public virtual void RemoveCard(TarotManager tarotManager, bool muted)
    {
        foreach (EffectInstance e in effectInstances)
        {
            tarotManager.effectManager.RemoveEffect(e, muted);
        }
        if (effectInstances.Count != 0 && !muted)
        {
            tarotManager.effectManager.ApplyEffects(); // Recalculate attributes
        }
        effectInstances.Clear();
        RemoveListeners(muted);
    }

    protected virtual void ApplyListenersEffects(bool muted = false) {}

    protected virtual void RemoveListeners(bool muted = false) {}
    

    protected override void GetLocalizedDesc()
    {
        desc = new LocalizedString
        {
            TableReference = "PastTarotTable"
        };

        SetTableEntries(arcana.ToString());
        
        SetDescriptionValues();
    }

    protected virtual void SetDescriptionValues()
    {
        // override in each child
    }

    protected override void SetTableEntries(string cardName)
    {
        desc.TableEntryReference = $"{cardName}Past";
    }
}
