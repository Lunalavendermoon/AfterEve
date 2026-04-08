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

        ClearAllCards(keepPast: false);
    }

    void OnEnable()
    {
        PlayerController.instance.playerInput.Player.UpdateTarotHand.performed += OnUpdateTarotPerformed; // for testing
        // PlayerController.instance.playerInput.Player.Attack.performed += OnMouseClick;

        DisplayHand();

        // AddCard(new HighPriestess_Present(1));
        // AddCard(new Chariot_Future(1));
        // AddCard(new Magician_Future(1));
        // AddCard(new Magician_Future(1));
        // AddCard(new Magician_Future(1));
    }

    void OnDisable()
    {
        PlayerController.instance.playerInput.Player.UpdateTarotHand.performed -= OnUpdateTarotPerformed;
        // PlayerController.instance.playerInput.Player.Attack.performed -= OnMouseClick;
    }

    public EffectManager effectManager;
    public TarotIcon tarotIcons;
    public Transform tarotHand;

    // present cards can stack, so we should store them according to their arcana in case we get duplicates
    public Dictionary<TarotCard.Arcana, Present_TarotCard> presentTarot = new();
    public List<Future_TarotCard> futureTarot = new();
    public Dictionary<TarotCard.Arcana, Past_TarotCard> pastTarot = new();

    public List<TarotUIScript> futureSkillUI = new();

    public void ClearAllCards(bool keepPast)
    {
        // Remove future cards (unhooks quest listeners)
        // PlayerController responsible for resetting future skill slots -- DON'T TOUCH PLAYER'S FUTURE SKILLS HERE
        for (int i = futureTarot.Count - 1; i >= 0; i--)
        {
            Future_TarotCard card = futureTarot[i];
            try
            {
                card?.RemoveCard(this);
            }
            finally
            {
                futureTarot.RemoveAt(i);
            }
        }

        // Remove present cards (removes applied effects)
        foreach (Present_TarotCard card in presentTarot.Values)
        {
            card?.RemoveCard(this);
        }
        presentTarot.Clear();

        if (!keepPast)
        {
            foreach (Past_TarotCard card in pastTarot.Values)
            {
                card?.RemoveCard(this);
            }
            pastTarot.Clear();
        }

        DisplayHand();
        // DisplayCards();
    }

    public void ClearOnPlayerDeath()
    {
        ClearAllCards(keepPast: true);
    }

    public static event Action<TarotCard.Arcana> OnObtainCard;

    // public void OnMouseClick(InputAction.CallbackContext context)
    // {
    //     RaycastHit hit;  
    //     if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.NameToLayer("TarotUI")))
    //     {
    //         Debug.Log("click");
        
    //         TarotUIScript card = hit.collider.gameObject.GetComponent<TarotUIScript>();
    //         if(card)
    //         {
    //             Debug.Log("card clicked");
    //             card.runTarotCooldownAnimation();
    //         }
    //     }
    // }

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
        else if (tarotCard is Future_TarotCard)
        {
            futureTarot.Remove((Future_TarotCard)tarotCard);
        }
        // DisplayCards();
    }

    public void Update()
    {
        // DisplayCards();

        // // testing
        // if(Input.GetKeyDown(KeyCode.O))
        // {
        //     // PlayerController.instance.futureSkills = futureTarot[0].reward;
        //     tarotHand.transform.GetChild(0).gameObject.GetComponent<TarotUIScript>().runTarotCooldownAnimation();
        // }
        // if(Input.GetKeyDown(KeyCode.P))
        // {
        //     // PlayerController.instance.futureSkills = futureTarot[1].reward;
        //     tarotHand.transform.GetChild(1).gameObject.GetComponent<TarotUIScript>().runTarotCooldownAnimation();
        // }

        // call Update() for any cards that need it
        if (pastTarot.ContainsKey(TarotCard.Arcana.Chariot))
        {
            pastTarot[TarotCard.Arcana.Chariot].UpdateCard();
        }
        if (presentTarot.ContainsKey(TarotCard.Arcana.Empress))
        {
            presentTarot[TarotCard.Arcana.Empress].UpdateCard();
        }
        if (presentTarot.ContainsKey(TarotCard.Arcana.Hierophant))
        {
            presentTarot[TarotCard.Arcana.Hierophant].UpdateCard();
        }
    }

    // // For testing
    // public void DisplayCards()
    // {
    //     string s = "Present: ";
        
    //     foreach (TarotCard present in presentTarot.Values)
    //     {
    //         s += present.cardName + " (" + present.quantity + ")\n";
    //     }
    //     s += "\nFuture: ";
    //     foreach (TarotCard future in futureTarot)
    //     {
    //         s += future.cardName + " (" + future.quantity + ") " + ((Future_TarotCard)future).GetQuestText() +
    //             (((Future_TarotCard)future).questCompleted ? " - DONE" : "") + "\n";
    //     }
    //     s += "\nPast: ";
    //     foreach (TarotCard past in pastTarot.Values)
    //     {
    //         s += past.cardName + "\n";
    //     }
    //     text.text = s;
    // }
    
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
        futureSkillUI.Clear();

        int offset = 200;
        int i = 0;
        foreach(Future_Reward future in PlayerController.instance.futureSkills)
        {
            if (future != null)
            {
                GameObject newCard = Instantiate(CreateCardObject(future.arcana), tarotHand);
                newCard.transform.position += new Vector3(i * offset, 0, 0);
                futureSkillUI.Add(newCard.GetComponent<TarotUIScript>());
            }
            else
            {
                // TODO - what to display for empty (i.e. null) skill slots?
                futureSkillUI.Add(null);
            }

            i++;
        }
    }

    public GameObject CreateCardObject(TarotCard.Arcana arcana)
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
        obj.GetComponent<Image>().sprite = tarotIcons.GetFutureSkillSprite(arcana);

        // add cooldown effect script
        obj.AddComponent<TarotUIScript>();
        
        return obj;
    }
}
