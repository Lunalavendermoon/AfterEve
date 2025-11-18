using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TarotManager : MonoBehaviour
{
    // for temp display
    public TMP_Text text;

    public static TarotManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public EffectManager effectManager;

    [SerializeField] List<Present_TarotCard> presentTarot = new List<Present_TarotCard>();

    [SerializeField] List<Future_TarotCard> futureTarot = new List<Future_TarotCard>();

    public void AddCard(TarotCard tarotCard)
    {
        if (tarotCard is Present_TarotCard)
        {
            presentTarot.Add((Present_TarotCard)tarotCard);
        }
        else if (tarotCard is Future_TarotCard)
        {
            futureTarot.Add((Future_TarotCard)tarotCard);
        }
        tarotCard.ApplyCard(this);
        DisplayCards();
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
        // I don't think we need this? bc Future cards remove themselves automatically
        // else if (tarotCard is Future_TarotCard)
        // {
        //     if (futureTarot.Remove((Future_TarotCard)tarotCard))
        //     {
        //         tarotCard.RemoveCard(this);
        //     }
        // }
        DisplayCards();
    }

    // Just for testing so that the future card quests update in real-time :o
    // Not super optimal, dont use this in the final version LOL
    public void Update()
    {
        DisplayCards();
    }

    public void DisplayCards()
    {
        string s = "Present: ";
        
        foreach (TarotCard present in presentTarot)
        {
            s += present.name + " ";
        }
        s += "\nFuture: ";
        foreach (TarotCard future in futureTarot)
        {
            s += future.name + " " + ((Future_TarotCard)future).GetQuestText() +
                (((Future_TarotCard)future).questCompleted ? " - DONE" : "") + "\n";
        }
        text.text = s;
    }

    void Start()
    {
        AddCard(new Chariot_Future(1));
        AddCard(new Empress_Present(1));
    }
}
