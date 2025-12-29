using UnityEngine;

public abstract class TarotCard : MonoBehaviour
{
    // Only used for shop RN, but feel free to use for tarot classes as well :)
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
    public new string name;
    public int quantity;

    public TarotCard(int q)
    {
        quantity = q;
    }

    public abstract void ApplyCard(TarotManager tarotManager);
    public abstract void RemoveCard(TarotManager tarotManager);
}
