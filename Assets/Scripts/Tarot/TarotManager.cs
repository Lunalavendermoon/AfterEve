using System.Collections.Generic;
using UnityEngine;

public class TarotManager : MonoBehaviour
{
    public static TarotManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public EffectManager effectManager = PlayerController.instance.gameObject.GetComponent<EffectManager>();

    List<Present_TarotCard> presentTarot = new List<Present_TarotCard>();

    public void AddCard(TarotCard tarotCard)
    {
        if (tarotCard is Present_TarotCard)
        {
            presentTarot.Add((Present_TarotCard)tarotCard);
        }
        tarotCard.ApplyCard(this);
    }

    public void RemoveCard(TarotCard tarotCard)
    {
        if (tarotCard is Present_TarotCard)
        {
            if (presentTarot.Remove((Present_TarotCard)tarotCard))
            {
                tarotCard.RemoveCard(this);
            }
        }
    }

    void Start()
    {

        foreach (Present_TarotCard present in presentTarot)
        {
            Debug.Log("name: " + present.name + " quantity: " + present.quantity);
        }
    }
}
