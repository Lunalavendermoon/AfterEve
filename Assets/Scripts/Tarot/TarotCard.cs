using UnityEngine;
using System;

public abstract class TarotCard
{
    public enum Arcana
    {
        Fool,
        Magician,
        HighPriestess,
        Empress,
        Emperor,
        Hierophant,
        Lovers,
        Chariot,
        Strength,
        Hermit,
        WheelOfFortune,
        Justice,
        HangedMan,
        Death,
        Temperance,
        Devil,
        Tower,
        Star,
        Moon,
        Sun,
        Judgement,
        World
    }
    public string cardName;
    public int quantity;

    public Arcana arcana;

    public TarotCard(int q)
    {
        quantity = q;
    }

    public TarotCard()
    {
        quantity = 1;
    }

    public abstract void ApplyCard(TarotManager tarotManager);
    public abstract void RemoveCard(TarotManager tarotManager);

    // Used to simulate Update() calls on tarot card without making them MonoBehavior
    public virtual void UpdateCard() {}

    public static TarotCard GetPresentFutureCard(TarotCard.Arcana arcana, bool isFuture, int quantity)
    {
        switch (arcana)
        {
            case TarotCard.Arcana.Fool:
                return isFuture ? new Fool_Future(quantity) : new Fool_Present(quantity);
            case TarotCard.Arcana.Magician:
                return isFuture ? new Magician_Future(quantity) : new Magician_Present(quantity);
            case TarotCard.Arcana.HighPriestess:
                return isFuture ? new HighPriestess_Future(quantity) : new HighPriestess_Present(quantity);
            case TarotCard.Arcana.Empress:
                return isFuture ? new Empress_Future(quantity) : new Empress_Present(quantity);
            case TarotCard.Arcana.Emperor:
                return isFuture ? new Emperor_Future(quantity) : new Emperor_Present(quantity);
            case TarotCard.Arcana.Hierophant:
                return isFuture ? new Hierophant_Future(quantity) : new Hierophant_Present(quantity);
            case TarotCard.Arcana.Lovers:
                return isFuture ? new Lovers_Future(quantity) : new Lovers_Present(quantity);
            case TarotCard.Arcana.Chariot:
                return isFuture ? new Chariot_Future(quantity) : new Chariot_Present(quantity);
            case TarotCard.Arcana.Strength:
                return isFuture ? new Strength_Future(quantity) : new Strength_Present(quantity);
            case TarotCard.Arcana.Hermit:
                return isFuture ? new Hermit_Future(quantity) : new Hermit_Present(quantity);
        }
        return null;
    }

    public static (Arcana, bool) GenRandomCardData()
    {
        bool future = UnityEngine.Random.Range(0, 2) == 0;

        Array values = Enum.GetValues(typeof(Arcana));
        int randomIndex = UnityEngine.Random.Range(0, values.Length);

        return ((Arcana)values.GetValue(randomIndex), future);
    }
}
