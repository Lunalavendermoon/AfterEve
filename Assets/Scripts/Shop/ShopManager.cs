using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public int shopSize = 6;

    public GameObject shopUi;

    public GameObject shopLayout;
    public GameObject shopItemPrefab;

    public Button toggleButton;
    public TMP_Text toggleText;
    public Button refreshButton;
    public TMP_Text refreshText;

    public List<GameObject> shopStock = new();

    int refreshCount;

    int[] normalPrice = new int[] { 7, 9, 13 };
    int[] discountPrice = new int[] { 2, 3, 5 };
    int[] refreshNormal = new int[] { 2, 2, 3 };
    int[] refreshDiscount = new int[] { 1, 1, 2 };

    int[] baseRefreshPrice = new int[] { 3, 4, 4 };
    int[] togglePrice = new int[] { 1, 2, 3 };

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        shopUi.SetActive(false);
        PlayerController.OnCoinsChange += OnCoinsChange;
    }

    public void DisableShopUI()
    {
        ShowShop(false);
    }

    public void ShowShop(bool enabled, bool firstTime = false)
    {
        if (enabled)
        {
            PlayerController.instance.DisablePlayerInput();
        }
        else
        {
            PlayerController.instance.EnablePlayerInput();
        }

        shopUi.SetActive(enabled);

        if (firstTime)
        {
            refreshCount = -1;
            shopStock.Clear();
            RefreshStock();
        }
    }

    public void RefreshStock()
    {
        ++refreshCount;

        if (refreshCount > 0)
        {
            PlayerController.instance.ChangeCoins(-GetRefreshCost());
        }

        // generate 2 random discounts without repetition
        List<int> numbers = new() { 0,1,2,3,4,5 };
        int index1 = UnityEngine.Random.Range(0, 6);
        int discount1 = numbers[index1];
        numbers.RemoveAt(index1);
        int index2 = UnityEngine.Random.Range(0, 5);
        int discount2 = numbers[index2];

        for (int i = 0; i < shopSize; ++i)
        {
            (TarotCard.Arcana, bool) card = TarotCard.GenRandomCardData();

            // TODO adjust these values accordingly if theyre unbalanced

            int price = ComputePrice(i == discount1 || i == discount2);
            
            // Random quantity from 1-5
            // Future cards ignore this quantity
            int quantity = UnityEngine.Random.Range(1, 6);

            GameObject go;
            if (shopStock.Count < shopSize)
            {
                go = Instantiate(shopItemPrefab, shopLayout.transform);
                shopStock.Add(go);
            }
            else
            {
                go = shopStock[i];
            }
            go.GetComponent<ShopItem>().InitializeShopItem(card.Item1, card.Item2, price, quantity);
        }

        refreshText.text = $"Refresh ({GetRefreshCost()})";
        toggleText.text = $"Swap Present-future ({GetToggleCost()})";
    }

    public void TogglePresentFuture()
    {
        foreach (GameObject go in shopStock)
        {
            go.GetComponent<ShopItem>().TogglePresentFuture();
        }
    }

    int GetIndexFromRoomCount()
    {
        int roomCount = NarrativeRoomManager.instance.roomCount;
        if (roomCount <= 3)
        {
            return 0;
        }
        else if (roomCount <= 6)
        {
            return 1;
        }
        return 2;
    }

    int ComputePrice(bool isDiscount)
    {
        int idx = GetIndexFromRoomCount();
        if (isDiscount)
        {
            return discountPrice[idx] + refreshCount * refreshDiscount[idx];
        }
        return normalPrice[idx] + refreshCount * refreshNormal[idx];
    }

    int GetRefreshCost()
    {
        return baseRefreshPrice[GetIndexFromRoomCount()] * (1 << refreshCount);
    }

    int GetToggleCost()
    {
        return togglePrice[GetIndexFromRoomCount()];
    }

    void OnCoinsChange()
    {
        refreshButton.enabled = PlayerController.instance.GetCoins() >= GetRefreshCost();
        toggleButton.enabled = PlayerController.instance.GetCoins() >= GetToggleCost();

        foreach (GameObject go in shopStock)
        {
            go.GetComponent<ShopItem>().SetButtonAvailable();
        }
    }
}