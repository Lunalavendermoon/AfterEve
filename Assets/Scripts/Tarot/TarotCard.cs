using UnityEngine;

public abstract class TarotCard : MonoBehaviour
{
    public string name;
    public int quantity;

    public TarotCard(string s, int q)
    {
        name = s;
        quantity = q;
    }

    public abstract void ApplyCard(TarotManager tarotManager);
    public abstract void RemoveCard(TarotManager tarotManager);
}
