using UnityEngine;

public class Fool_Past : Past_TarotCard
{
    public const float physicalDmgBonus = 0.07f;
    const int maxStackPerFoolCard = 7;
    int foolCardCount = 1;
    int stackCounter = 0;

    public Fool_Past() : base()
    {
        effectInstances.Add(null);

        cardName = "Fool_Past";
        arcana = Arcana.Fool;

        GetLocalizedDesc();
    }

    protected override void ApplyListenersEffects(bool muted = false)
    {
        TarotManager.OnObtainCard += OnObtainCard;
    }

    protected override void RemoveListeners(bool muted = false)
    {
        TarotManager.OnObtainCard -= OnObtainCard;
    }

    void OnObtainCard(Arcana arcana)
    {
        if (arcana == Arcana.Fool)
        {
            foolCardCount += 1;
        }

        if (stackCounter >= maxStackPerFoolCard * foolCardCount)
        {
            return;
        }
        
        ++stackCounter;

        EffectManager em = PlayerController.instance.gameObject.GetComponent<EffectManager>();
        if (effectInstances[0] != null)
        {
            em.RemoveEffect(effectInstances[0], true);
        }
        effectInstances[0] = em.AddBuff(new PhysicalDmgBonus_Effect(-1, physicalDmgBonus * stackCounter));
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            FormatPercentage(physicalDmgBonus),
            FormatPercentage(physicalDmgBonus * maxStackPerFoolCard)
        };
    }
}