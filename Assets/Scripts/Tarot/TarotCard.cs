using UnityEngine;

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

    public abstract void ApplyCard(TarotManager tarotManager);
    public abstract void RemoveCard(TarotManager tarotManager);
}
