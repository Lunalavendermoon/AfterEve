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

    // present cards can stack, so we should store them according to their arcana in case we get duplicates
    [SerializeField] Dictionary<string, int> presentDict = new();
    [SerializeField] List<Present_TarotCard> presentTarot = new();
    [SerializeField] List<Future_TarotCard> futureTarot = new();

    public void AddCard(TarotCard tarotCard)
    {
        if (tarotCard is Present_TarotCard)
        {
            presentTarot.Add((Present_TarotCard)tarotCard);
            if (!presentDict.TryAdd(tarotCard.cardName, tarotCard.quantity)) {
                presentDict[tarotCard.cardName] += tarotCard.quantity;
            }
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
            if (!presentTarot.Remove((Present_TarotCard)tarotCard))
            {
                // card does not exist in tarotmanager
                return;
            }

            presentDict[tarotCard.cardName] -= tarotCard.quantity;
            tarotCard.RemoveCard(this);

            if (presentDict[tarotCard.cardName] == 0)
            {
                presentDict.Remove(tarotCard.cardName);
            }
        }
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
            s += present.cardName + " (" + present.quantity + ")\n";
        }
        s += "\nFuture: ";
        foreach (TarotCard future in futureTarot)
        {
            s += future.cardName + " (" + future.quantity + ") " + ((Future_TarotCard)future).GetQuestText() +
                (((Future_TarotCard)future).questCompleted ? " - DONE" : "") + "\n";
        }
        text.text = s;
    }

    void Start()
    {
        AddCard(new Chariot_Future(1));
        AddCard(new Lovers_Future(1));
        AddCard(new Empress_Present(1));
    }
}
