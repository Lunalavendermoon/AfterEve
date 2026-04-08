using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;

public class Present_TarotCard : TarotCard
{
    int[] levelThreshold = {1, 2, 4, 8, 16};

    public List<Effects> effects = new();
    public List<EffectInstance> effectInstances = new();
    public int level;

    public Present_TarotCard(int q) : base(q)
    {
        for (int i = levelThreshold.Length - 1; i >= 0; --i)
        {
            if (quantity >= levelThreshold[i])
            {
                level = i;
                break;
            }
        }
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        if (effects != null) {
            foreach (Effects e in effects)
            {
                effectInstances.Add(
                    tarotManager.effectManager.AddEffect(e, PlayerController.instance.playerAttributes, true));
            }
            tarotManager.effectManager.ApplyEffects(); // Recalculate attributes
        }
        ApplyListeners();
    }

    public override void RemoveCard(TarotManager tarotManager)
    {
        if (effectInstances != null) {
            foreach (EffectInstance e in effectInstances)
            {
                tarotManager.effectManager.RemoveEffect(e, true);
            }
            tarotManager.effectManager.ApplyEffects(); // Recalculate attributes
            effectInstances.Clear();
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

        SetTableEntries(arcana.ToString());
        
        SetDescriptionValues();
    }

    protected virtual void SetDescriptionValues()
    {
        // override in each child
    }

    public void ChangeQuantity(int q)
    {
        quantity += q;
        RecomputeCardLevel();
    }

    public void RecomputeCardLevel()
    {
        // 1,2,4,8,16 is level 1,2,3,4,5
        int oldLevel = level;

        for (int i = levelThreshold.Length - 1; i >= 0; --i)
        {
            if (quantity >= levelThreshold[i])
            {
                level = i;
                break;
            }
        }
        Debug.Log($"### {quantity} {level} {oldLevel}");

        if (level != oldLevel)
        {
            SetDescriptionValues();
            desc.RefreshString();
            OnCardLevelUp();
        }
    }

    protected virtual void OnCardLevelUp()
    {
        if (effects.Count == 0)
        {
            return;
        }
        
        foreach (EffectInstance e in effectInstances)
        {
            TarotManager.instance.effectManager.RemoveEffect(e, true);
        }
        effectInstances.Clear();

        effects.Clear();
        AddNewLevelEffects();

        foreach (Effects e in effects)
        {
            effectInstances.Add(
                TarotManager.instance.effectManager.AddEffect(e, PlayerController.instance.playerAttributes, true));
        }

        TarotManager.instance.effectManager.ApplyEffects(); // Recalculate attributes
    }

    protected virtual void AddNewLevelEffects() {}

    protected override void SetTableEntries(string cardName)
    {
        desc.TableEntryReference = $"{cardName}Present";
    }
}
