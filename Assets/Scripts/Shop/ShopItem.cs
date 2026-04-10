using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    TarotCard.Arcana arcana;
    TarotCard.TarotType tarotType;
    int price;
    int quantity;

    Dictionary<TarotCard.TarotType, bool> purchased = new();

    public TMP_Text itemName;
    public Button purchaseButton;
    public TMP_Text purchaseButtonText;

    public void InitializeShopItem(TarotCard.Arcana arcana, TarotCard.TarotType tarotType, int price, int quantity)
    {
        this.arcana = arcana;
        this.tarotType = tarotType;
        this.price = price;
        this.quantity = quantity;

        purchased[TarotCard.TarotType.Past] = false;
        purchased[TarotCard.TarotType.Present] = false;
        purchased[TarotCard.TarotType.Future] = false;

        ResetDisplay();
    }

    void ResetDisplay()
    {
        itemName.text = $"{arcana} {tarotType} ({quantity}) - {price} coins";
        purchaseButtonText.text = purchased[tarotType] ? "(Purchased)" : "Buy";
        SetButtonAvailable();
    }

    public void PurchaseItem()
    {
        if (purchased[tarotType] || PlayerController.instance.GetCoins() < price)
        {
            return;
        }

        PlayerController.instance.ChangeCoins(-price, true);

        purchased[tarotType] = true;

        purchaseButton.enabled = false;
        purchaseButtonText.text = "(Purchased)";

        TarotCard card = TarotCard.GetCardFromData(arcana, tarotType, quantity);

        // TODO - can remove the null check after all tarot cards are implemented
        // currently return null as placeholder for unimplemented cards
        if (card != null)
        {
            TarotManager.instance.AddCard(card);
        }
    }

    public void TogglePastFuture()
    {
        // TODO - can past cards stack? if not, then we should disable button if past card of this arcana is purchased
        if (tarotType == TarotCard.TarotType.Future)
        {
            tarotType = TarotCard.TarotType.Past;
        }
        else if (tarotType == TarotCard.TarotType.Past)
        {
            tarotType = TarotCard.TarotType.Future;
        }
        else
        {
            return;
        }

        ResetDisplay();
    }

    public void SetButtonAvailable()
    {
        if (purchased[tarotType])
        {
            purchaseButton.enabled = false;
            return;
        }
        purchaseButton.enabled = PlayerController.instance.GetCoins() >= price;
    }
}