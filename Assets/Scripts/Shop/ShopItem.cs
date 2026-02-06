using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    TarotCard.Arcana arcana;
    bool isFuture;
    int price;
    int quantity;

    Dictionary<bool, bool> purchased = new();

    public TMP_Text itemName;
    public Button purchaseButton;
    public TMP_Text purchaseButtonText;

    public void InitializeShopItem(TarotCard.Arcana arcana, bool isFuture, int price, int quantity)
    {
        this.arcana = arcana;
        this.isFuture = isFuture;
        this.price = price;
        this.quantity = quantity;

        purchased[true] = false;
        purchased[false] = false;

        ResetDisplay();
    }

    void ResetDisplay()
    {
        itemName.text = $"{arcana} {(isFuture ? "Future" : "Present")} ({quantity}) - {price} coins";
        purchaseButtonText.text = purchased[isFuture] ? "(Purchased)" : "Buy";
        SetButtonAvailable();
    }

    public void PurchaseItem()
    {
        if (purchased[isFuture] || PlayerController.instance.GetCoins() < price)
        {
            return;
        }

        Debug.Log($"Purchased item {arcana} {(isFuture ? "Future" : "Present")} ({quantity}) for {price} coins");

        PlayerController.instance.ChangeCoins(-price, true);

        purchased[isFuture] = true;

        purchaseButton.enabled = false;
        purchaseButtonText.text = "(Purchased)";

        // TODO - can remove the null check after all tarot cards are implemented
        // currently return null as placeholder for unimplemented cards
        TarotCard card = TarotCard.GetPresentFutureCard(arcana, isFuture, quantity);
        if (card != null)
        {
            PlayerController.instance.tarotManager.AddCard(card);
        }
    }

    public void TogglePresentFuture()
    {
        isFuture = !isFuture;
        ResetDisplay();
    }

    public void SetButtonAvailable()
    {
        if (purchased[isFuture])
        {
            purchaseButton.enabled = false;
            return;
        }
        purchaseButton.enabled = PlayerController.instance.GetCoins() >= price;
    }
}