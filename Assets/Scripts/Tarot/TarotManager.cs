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

        // AddCard(new Hierophant_Past(1));
        // AddCard(new Fool_Past(1));
        // AddCard(new Fool_Present(2));
        // AddCard(new Empress_Present(1));

        AddCard(new Chariot_Present(1));
        AddCard(new Emperor_Present(1));
        AddCard(new Empress_Present(1));
        AddCard(new Hermit_Present(1));
        AddCard(new Hierophant_Present(1));
        AddCard(new HighPriestess_Present(1));
        AddCard(new Lovers_Present(1));
        AddCard(new Magician_Present(1));
        AddCard(new Strength_Present(1));
        AddCard(new Fool_Present(1));

        // TODO - uncomment this (only commented out for testing)
        // DisplayHand();
    }

    void OnDisable()
    {
        PlayerController.instance.playerInput.Player.UpdateTarotHand.performed -= OnUpdateTarotPerformed;
        PlayerController.instance.playerInput.Player.Attack.performed -= OnMouseClick;
    }

    public EffectManager effectManager;
    public TarotIcon tarotIcons;
    public Transform tarotHand;

    // present cards can stack, so we should store them according to their arcana in case we get duplicates
    public Dictionary<TarotCard.Arcana, Present_TarotCard> presentTarot = new();
    public List<Future_TarotCard> futureTarot = new();
    public Dictionary<TarotCard.Arcana, Past_TarotCard> pastTarot = new();

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
        bool applyCard = true;
        if (tarotCard is Present_TarotCard)
        {
            if (!presentTarot.TryAdd(tarotCard.arcana, (Present_TarotCard)tarotCard)) {
                presentTarot[tarotCard.arcana].ChangeQuantity(tarotCard.quantity);
                applyCard = false;
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
        if (applyCard)
        {
            tarotCard.ApplyCard(this);
        }
        OnObtainCard?.Invoke(tarotCard.arcana);
    }

    public void RemoveCard(TarotCard tarotCard)
    {
        if (tarotCard is Present_TarotCard)
        {
            if (!presentTarot.ContainsKey(tarotCard.arcana))
            {
                // card does not exist in tarotmanager
                return;
            }

            presentTarot[tarotCard.arcana].ChangeQuantity(-tarotCard.quantity);

            if (presentTarot[tarotCard.arcana].quantity <= 0)
            {
                presentTarot[tarotCard.arcana].RemoveCard(this);
                presentTarot.Remove(tarotCard.arcana);
            }
        }
        DisplayCards();
    }

    // Just for testing so that the future card quests update in real-time :o
    // Not super optimal, dont use this in the final version LOL
    public void Update()
    {
        DisplayCards();

        // testing
        if(Input.GetKeyDown(KeyCode.O))
        {
            PlayerController.instance.futureSkill = futureTarot[0].reward;
            tarotHand.transform.GetChild(0).gameObject.GetComponent<TarotUIScript>().runTarotCooldownAnimation();
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayerController.instance.futureSkill = futureTarot[1].reward;
            tarotHand.transform.GetChild(1).gameObject.GetComponent<TarotUIScript>().runTarotCooldownAnimation();
        }

        // call Update() for any cards that need it -- currently only chariot past
        if (pastTarot.ContainsKey(TarotCard.Arcana.Chariot))
        {
            pastTarot[TarotCard.Arcana.Chariot].UpdateCard();
        }
        if (presentTarot.ContainsKey(TarotCard.Arcana.Empress))
        {
            presentTarot[TarotCard.Arcana.Empress].UpdateCard();
        }
    }

    // For testing
    public void DisplayCards()
    {
        string s = "Present: ";
        
        foreach (TarotCard present in presentTarot.Values)
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
        // set random hand (for testing)
        TarotCard[] randomFutureCards = {new Chariot_Future(1), new Emperor_Future(1), new Hierophant_Future(1), new Lovers_Future(1), new Strength_Future(1), new Magician_Future(1)};
        futureTarot.Clear();
        for(int id=0;id<5;id++)
        {
            AddCard(randomFutureCards[UnityEngine.Random.Range(0, randomFutureCards.Length-1)]);
        }

        Debug.Log("updated hand");
        DisplayHand();
    }

    public void DisplayHand()
    {
        // clear previous hand
        foreach(Transform child in tarotHand)
        {
            Destroy(child.gameObject);
        }

        int offset = 200;
        int i = 0;
        foreach(TarotCard future in futureTarot)
        {
            GameObject newCard = Instantiate(CreateCardObject(future), tarotHand);
            newCard.transform.position += new Vector3(i * offset, 0, 0);
            i++;
        }
    }

    public GameObject CreateCardObject(TarotCard card)
    {
        GameObject obj = new GameObject();

        // dimensions of sprite
        int width = 750;
        int height = 1283;

        float scalar = 0.25f; // I just did something random that looks good

        // set size
        obj.AddComponent<RectTransform>();
        obj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width*scalar);
        obj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height*scalar);

        // add sprite image
        obj.AddComponent<Image>();
        obj.GetComponent<Image>().sprite = tarotIcons.GetSprite(card);

        // add cooldown effect script
        obj.AddComponent<TarotUIScript>();
        
        return obj;
    }
}
