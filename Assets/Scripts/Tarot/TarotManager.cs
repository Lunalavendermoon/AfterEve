using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TarotManager : MonoBehaviour
{
    // for temp display
    public TMP_Text text;

    public static TarotManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void OnEnable()
    {
        PlayerController.instance.playerInput.Player.UpdateTarotHand.performed += OnUpdateTarotPerformed; // for testing
        PlayerController.instance.playerInput.Player.Attack.performed += OnMouseClick;

        // commented some cards out for testing purposes, uncomment if u need to use them :D

        AddCard(new Hierophant_Past(1));
        AddCard(new Chariot_Future(1));
        AddCard(new Lovers_Future(1));
        AddCard(new Empress_Present(1));

        // DisplayHand();
    }

    void OnDisable()
    {
        PlayerController.instance.playerInput.Player.UpdateTarotHand.performed -= OnUpdateTarotPerformed;
        PlayerController.instance.playerInput.Player.Attack.performed -= OnMouseClick;
    }

    public EffectManager effectManager;
    public TarotIcon tarotIcons;
    public GameObject tarotHand;
    public GameObject tarotPrefab;

    // present cards can stack, so we should store them according to their arcana in case we get duplicates
    [SerializeField] Dictionary<string, int> presentDict = new();
    [SerializeField] List<Present_TarotCard> presentTarot = new();
    [SerializeField] List<Future_TarotCard> futureTarot = new();
    [SerializeField] Dictionary<TarotCard.Arcana, Past_TarotCard> pastTarot = new();

    public static event Action<TarotCard.Arcana> OnObtainCard;

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        RaycastHit hit;  
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.NameToLayer("TarotUI")))
        {
            Debug.Log("click");
        
            TarotUIScript card = hit.collider.gameObject.GetComponent<TarotUIScript>();
            if(card)
            {
                Debug.Log("card clicked");
                card.runTarotCooldownAnimation();
            }
        }
    }

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
        else if (tarotCard is Past_TarotCard)
        {
            if (pastTarot.ContainsKey(tarotCard.arcana))
            {
                return;
            }
            pastTarot.Add(tarotCard.arcana, (Past_TarotCard)tarotCard);
        }
        tarotCard.ApplyCard(this);
        OnObtainCard?.Invoke(tarotCard.arcana);
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

        if(Input.GetKeyDown(KeyCode.I))
        {
            PlayerController.instance.futureSkill = futureTarot[0].reward;
            tarotHand.transform.GetChild(0).gameObject.GetComponent<TarotUIScript>().runTarotCooldownAnimation();
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            PlayerController.instance.futureSkill = futureTarot[1].reward;
            tarotHand.transform.GetChild(1).gameObject.GetComponent<TarotUIScript>().runTarotCooldownAnimation();
        }

        // call Update() for any cards that need it -- currently only chariot past
        if (pastTarot.ContainsKey(TarotCard.Arcana.Chariot))
        {
            pastTarot[TarotCard.Arcana.Chariot].UpdateCard();
        }
    }

    // For testing
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
        s += "\nPast: ";
        foreach (TarotCard past in pastTarot.Values)
        {
            s += past.cardName + "\n";
        }
        text.text = s;
    }
    
    // testing future tarot card hand
    public void OnUpdateTarotPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("updated hand");
        DisplayHand();
    }

    public void DisplayHand()
    {
        // set random hand (for testing)
        TarotCard[] randomFutureCards = {new Chariot_Future(1), new Emperor_Future(1), new Hierophant_Future(1), new Lovers_Future(1), new Strength_Future(1), new Magician_Future(1)};
        futureTarot.Clear();
        for(int id=0;id<5;id++)
        {
            AddCard(randomFutureCards[UnityEngine.Random.Range(0, randomFutureCards.Length-1)]);
        }

        // clear previous hand
        foreach(GameObject child in tarotHand.transform)
        {
            Destroy(child);
        }

        int offset = 200;
        int i = 0;
        foreach(TarotCard future in futureTarot)
        {
            GameObject newCard = Instantiate(CreateCardObject(future), tarotHand.transform);
            newCard.transform.position += new Vector3(i * offset, 0, 0);
            i++;
        }
    }

    public GameObject CreateCardObject(TarotCard card)
    {
        GameObject obj = tarotPrefab;
        
        // establish if card is past, present, or future when fetching corresponding Sprite
        TarotIcon.TarotType type = TarotIcon.TarotType.Past;
        if(card is Present_TarotCard) type = TarotIcon.TarotType.Present;
        else if(card is Future_TarotCard) type = TarotIcon.TarotType.Future;

        obj.GetComponent<Image>().sprite = tarotIcons.GetSprite(card.arcana, type);
        
        return obj;
    }
}
