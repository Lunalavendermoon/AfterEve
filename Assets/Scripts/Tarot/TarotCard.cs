using UnityEngine;
using System;
using UnityEngine.Localization;

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

    public enum TarotType
    {
        Past = 0,
        Present = 1,
        Future = 2
    }

    public string cardName;
    public int quantity;

    public Arcana arcana;
    public TarotType tarotType;

    protected LocalizedString desc;
    
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

    public static TarotCard GetCardFromData(Arcana arcana, TarotType type, int quantity = 1)
    {
        switch (arcana)
        {
            case Arcana.Fool:
                return type == TarotType.Future ? new Fool_Future() : (type == TarotType.Present ? new Fool_Present(quantity) : new Fool_Past());
            case Arcana.Magician:
                return type == TarotType.Future ? new Magician_Future() : (type == TarotType.Present ? new Magician_Present(quantity) : new Magician_Past());
            case Arcana.HighPriestess:
                return type == TarotType.Future ? new HighPriestess_Future() : (type == TarotType.Present ? new HighPriestess_Present(quantity) : new HighPriestess_Past());
            case Arcana.Empress:
                return type == TarotType.Future ? new Empress_Present(1) : (type == TarotType.Present ? new Empress_Present(quantity) : new Empress_Past());
            case Arcana.Emperor:
                return type == TarotType.Future ? new Emperor_Future() : (type == TarotType.Present ? new Emperor_Present(quantity) : new Emperor_Past());
            case Arcana.Hierophant:
                return type == TarotType.Future ? new Hierophant_Future() : (type == TarotType.Present ? new Hierophant_Present(quantity) : new Hierophant_Past());
            case Arcana.Lovers:
                return type == TarotType.Future ? new Lovers_Future() : (type == TarotType.Present ? new Lovers_Present(quantity) : new Lovers_Past());
            case Arcana.Chariot:
                return type == TarotType.Future ? new Chariot_Future() : (type == TarotType.Present ? new Chariot_Present(quantity) : new Chariot_Past());
            case Arcana.Strength:
                return type == TarotType.Future ? new Strength_Future() : (type == TarotType.Present ? new Strength_Present(quantity) : new Strength_Past());
            case Arcana.Hermit:
                return type == TarotType.Future ? new Hermit_Future() : (type == TarotType.Present ? new Hermit_Present(quantity) : new Hermit_Past());
        }
        return null;
    }

    public static (Arcana, bool) GenRandomCardData()
    {
        bool future = UnityEngine.Random.Range(0, 2) == 0;

        Array values = Enum.GetValues(typeof(Arcana));
        // int randomIndex = UnityEngine.Random.Range(0, values.Length);
        int randomIndex = UnityEngine.Random.Range(0, 10); // FOR TESTING - only generate first 10 card types

        return ((Arcana)values.GetValue(randomIndex), future);

        // FOR TESTING ONLY - force game to generate a specific present/future card
        // 2nd value: false for present, true for future
        // return (Arcana.Hierophant, false);
    }

    protected virtual void GetLocalizedDesc()
    {
        // TODO - override this for each tarot card
    }

    protected abstract void SetTableEntries(string cardName);

    public virtual string GetDescription()
    {
        return desc.GetLocalizedString();
    }

    public static int FormatPercentage(float amount)
    {
        return Mathf.RoundToInt(amount * 100);
    }

    public static int FormatPlusOnePercentage(float amount)
    {
        return Mathf.RoundToInt(amount * 100 - 100);
    }

    // public static int FormatDecreasePercentage(float amount)
    // {
    //     return Mathf.RoundToInt(100 - amount * 100);
    // }

    public static int Rnd(float amount)
    {
        return Mathf.RoundToInt(amount);
    }
}
