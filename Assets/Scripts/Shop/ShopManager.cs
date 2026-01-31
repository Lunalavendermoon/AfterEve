using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public int shopSize = 6;

    public GameObject shopUi;

    public GameObject shopLayout;
    public GameObject shopItemPrefab;

    public List<GameObject> shopStock = new();

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
            PlayerController.instance.DisablePlayerInput();
        }
        else
        {
            PlayerController.instance.EnablePlayerInput();
        }

        shopUi.SetActive(enabled);

        if (refreshStock)
        {
            shopStock.Clear();

            for (int i = 0; i < shopSize; ++i)
            {
                (TarotCard.Arcana, bool) card = TarotCard.GenRandomCardData();

                // TODO adjust these values accordingly if theyre unbalanced

                // Random price from 25-100 coins
                int price = UnityEngine.Random.Range(25, 101);
                
                // Random quantity from 1-5
                // Future cards can only generate w/ quantity of 1
                int quantity = card.Item2 ? 1 : UnityEngine.Random.Range(1, 6);

                GameObject go = Instantiate(shopItemPrefab, shopLayout.transform);
                shopStock.Add(go);
                go.GetComponent<ShopItem>().InitializeShopItem(card.Item1, card.Item2, price, quantity);
            }
        }
    }
}