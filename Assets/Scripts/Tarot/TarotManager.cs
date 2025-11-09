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


}
