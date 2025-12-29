using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public int shopSize = 6;

    public GameObject shopUi;

    public TMP_Text testingText;

    // tarot card, isFuture, cost in coins
    public List<((TarotCard.Arcana, bool), int)> shopStock = new();

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        shopUi.SetActive(false);
    }

    public void DisableShopUI()
    {
        ShowShop(false);
    }

    public void ShowShop(bool enabled, bool refreshStock = false)
    {
        if (enabled)
        {
            PlayerController.instance.playerInput.Disable();
        }
        else
        {
            PlayerController.instance.playerInput.Enable();
        }

        shopUi.SetActive(enabled);

        if (refreshStock)
        {
            shopStock.Clear();
            testingText.text = "";

            for (int i = 0; i < shopSize; ++i)
            {
                // Random price from 25-100 coins
                shopStock.Add((GenRandomCard(), UnityEngine.Random.Range(25, 101)));
                testingText.text += $"\n{shopStock[i].Item1.Item1} {(shopStock[i].Item1.Item2 ? "Future" : "Present")} - {shopStock[i].Item2} Coins";
            }
        }
    }

    (TarotCard.Arcana, bool) GenRandomCard()
    {
        bool future = UnityEngine.Random.Range(0, 2) == 0;

        Array values = Enum.GetValues(typeof(TarotCard.Arcana));
        int randomIndex = UnityEngine.Random.Range(0, values.Length);

        return ((TarotCard.Arcana)values.GetValue(randomIndex), future);
    }
}