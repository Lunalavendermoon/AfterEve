using UnityEngine;

public class Magician_Present : Present_TarotCard
{
    int[] bounceNum = {3,3,4,4,5};
    float[] damageReducedPerBounce = {.3f, .25f, .2f, .1f, 0f};
    
    public Magician_Present(int q) : base(q)
    {
        cardName = "Magician_Present";
        arcana = Arcana.Magician;

        AddNewLevelEffects();

        GetLocalizedDesc();
    }

    protected override void AddNewLevelEffects()
    {
        // Conditional: Only add effect if this card is higher level than Strength
        if (!TarotManager.instance.presentTarot.ContainsKey(Arcana.Strength) ||
            TarotManager.instance.presentTarot[Arcana.Strength].level < level)
        {
            effects.Add(new MagicianPresent_Effect(bounceNum[level], damageReducedPerBounce[level]));

            // Delete Strength effect if it exists
            if (TarotManager.instance.presentTarot.TryGetValue(Arcana.Strength, out var strengthCard))
            {
                ((Strength_Present)strengthCard).DisableEffect();
            }
        }
    }

    public void DisableEffect()
    {
        foreach (var ei in effectInstances)
        {
            TarotManager.instance.effectManager.RemoveEffect(ei);
        }
        effectInstances.Clear();
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[] {
            bounceNum[level],
            FormatPercentage(damageReducedPerBounce[level])
        };
    }
}
