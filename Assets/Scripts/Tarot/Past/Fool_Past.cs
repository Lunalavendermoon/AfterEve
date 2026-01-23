using UnityEngine;

public class Fool_Past : Past_TarotCard
{
    public const float physicalDmgBonus = 0.07f;
    const int maxStackPerFoolCard = 7;
    int foolCardCount = 1;
    int stackCounter = 0;

    public Fool_Past(int q) : base(q)
    {
        cardName = "Fool_Past";
        arcana = Arcana.Fool;
    }

    protected override void ApplyListeners()
    {
        TarotManager.OnObtainCard += OnObtainCard;
    }

    void OnObtainCard(Arcana arcana)
    {
        Debug.Log($"OBTAINED CARD {arcana}");
        if (arcana == Arcana.Fool)
        {
            foolCardCount += 1;
        }

        if (stackCounter >= maxStackPerFoolCard * foolCardCount)
        {
            return;
        }

        ++stackCounter;
        PlayerController.instance.gameObject.GetComponent<EffectManager>().AddEffect(new FoolPast_Effect());
    }
}