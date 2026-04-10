using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public int shopSize;

    public GameObject shopUi;

    public GameObject shopLayout;
    public GameObject shopItemPrefab;

    public Button toggleButton;
    public TMP_Text toggleText;
    public Button refreshButton;
    public TMP_Text refreshText;
    public Button luckButton;
    public TMP_Text luckText;
    public Button skillSlotButton;
    public TMP_Text skillSlotText;

    public List<GameObject> shopStock = new();

    int refreshCount;

    bool firstInteraction = true;

    int[] normalPrice = new int[] { 7, 9, 13 };
    int[] discountPrice = new int[] { 2, 3, 5 };
    int[] refreshNormal = new int[] { 2, 2, 3 };
    int[] refreshDiscount = new int[] { 1, 1, 2 };

    int[] baseRefreshPrice = new int[] { 3, 4, 4 };
    int[] togglePrice = new int[] { 1, 2, 3 };

    int currentLuckCost = 0;

    bool openedFromStory;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        shopUi.SetActive(false);
    }

    public void RefreshLuckPrice()
    {
        int luck = (int)PlayerController.instance.playerAttributes.GetAdjustedLuck();
        currentLuckCost = luck * 12 - luck * UnityEngine.Random.Range(0, 2);
    }

    public void DisableShopUI()
    {
        ShowShop(false);
    }

    public void ShowShop(bool enabled)
    {
        SetShopEnabled(enabled, false);
    }

    public void SetShopEnabled(bool enabled, bool fromStory)
    {
        if (enabled)
        {
            openedFromStory = fromStory;
            if (!fromStory)
            {
                PlayerController.instance.DisablePlayerInput();
            }
            PlayerController.OnCoinsChange += SetButtonsAvailable;
        }
        else
        {
            if (!openedFromStory)
            {
                // Don't re-enable player input if shop was opened from yarn
                // NarrativeRoomManager will re-enable input once the time is right :)
                PlayerController.instance.EnablePlayerInput();
            }
            PlayerController.OnCoinsChange -= SetButtonsAvailable;
        }

        shopUi.SetActive(enabled);

        if (firstInteraction)
        {
            firstInteraction = false;

            shopStock.Clear();
            refreshCount = -1;
            RefreshStock();

            refreshText.text = $"Refresh ({GetRefreshCost()})";
            toggleText.text = $"Swap Past-future ({GetToggleCost()})";
            luckText.text = $"Luck Coin ({GetLuckCost()})";
            skillSlotText.text = $"Skill Slot ({GetSkillSlotCost()})";

            SetButtonsAvailable();
        }
    }

    public bool ShopIsClosed()
    {
        return !shopUi.activeSelf;
    }

    public void RefreshStock()
    {
        ++refreshCount;

        // generate 2 random discounts without repetition
        List<int> numbers = new() { 0,1,2,3,4,5 };
        int index1 = UnityEngine.Random.Range(0, 6);
        int discount1 = numbers[index1];
        numbers.RemoveAt(index1);
        int index2 = UnityEngine.Random.Range(0, 5);
        int discount2 = numbers[index2];

        for (int i = 0; i < shopSize; ++i)
        {
            (TarotCard.Arcana, TarotCard.TarotType) card = TarotCard.GenRandomCardData();

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

        if (refreshCount > 0)
        {
            PlayerController.instance.ChangeCoins(-GetRefreshCost());
        }
    }

    public void TogglePastFuture()
    {
        foreach (GameObject go in shopStock)
        {
            go.GetComponent<ShopItem>().TogglePastFuture();
        }
        PlayerController.instance.ChangeCoins(-GetToggleCost());
    }

    public void BuyLuckCoin()
    {
        ++StaticGameManager.luckyCoins;
        PlayerController.instance.ChangeCoins(-GetLuckCost());
    }

    public void BuySkillSlot()
    {
        PlayerController.instance.GainFutureSkillSlot(1);
        skillSlotText.text = $"Skill Slot ({GetSkillSlotCost()})";
    }

    int GetIndexFromRoomCount()
    {
        int roomCount = StaticGameManager.roomCount;
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

    int GetLuckCost()
    {
        return currentLuckCost;
    }

    int GetSkillSlotCost()
    {
        // Price = 2 ^ (current # skill slots)
        return 1 << StaticGameManager.futureSkillSlots;
    }

    void SetButtonsAvailable()
    {
        refreshButton.enabled = PlayerController.instance.GetCoins() >= GetRefreshCost();
        
        toggleButton.enabled = PlayerController.instance.GetCoins() >= GetToggleCost();
        
        luckButton.enabled = StaticGameManager.luckyCoins < StaticGameManager.maxLuckyCoins &&
            PlayerController.instance.GetCoins() >= GetLuckCost();
        
        skillSlotButton.enabled = StaticGameManager.futureSkillSlots < StaticGameManager.maxSkillSlots &&
            PlayerController.instance.GetCoins() >= GetSkillSlotCost();

        foreach (GameObject go in shopStock)
        {
            go.GetComponent<ShopItem>().SetButtonAvailable();
        }
    }
}