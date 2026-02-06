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

    public static TarotCard GetPresentFutureCard(Arcana arcana, bool isFuture, int quantity)
    {
        switch (arcana)
        {
            case Arcana.Fool:
                return isFuture ? new Fool_Future(1) : new Fool_Present(quantity);
            case Arcana.Magician:
                return isFuture ? new Magician_Future(1) : new Magician_Present(quantity);
            case Arcana.HighPriestess:
                return isFuture ? new HighPriestess_Future(1) : new HighPriestess_Present(quantity);
            case Arcana.Empress:
                return isFuture ? new Empress_Future(1) : new Empress_Present(quantity);
            case Arcana.Emperor:
                return isFuture ? new Emperor_Future(1) : new Emperor_Present(quantity);
            case Arcana.Hierophant:
                return isFuture ? new Hierophant_Future(1) : new Hierophant_Present(quantity);
            case Arcana.Lovers:
                return isFuture ? new Lovers_Future(1) : new Lovers_Present(quantity);
            case Arcana.Chariot:
                return isFuture ? new Chariot_Future(1) : new Chariot_Present(quantity);
            case Arcana.Strength:
                return isFuture ? new Strength_Future(1) : new Strength_Present(quantity);
            case Arcana.Hermit:
                return isFuture ? new Hermit_Future(1) : new Hermit_Present(quantity);
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
