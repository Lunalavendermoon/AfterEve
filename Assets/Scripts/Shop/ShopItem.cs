using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    TarotCard.Arcana arcana;
    bool isFuture;
    int price;
    int quantity;

    bool purchased;

    public TMP_Text itemName;
    public Button purchaseButton;
    public TMP_Text purchaseButtonText;

    public void InitializeShopItem(TarotCard.Arcana arcana, bool isFuture, int price, int quantity)
    {
        this.arcana = arcana;
        this.isFuture = isFuture;
        this.price = price;
        this.quantity = quantity;

        itemName.text = $"{arcana} {(isFuture ? "Future" : "Present")} ({quantity}) - {price} coins";
        purchaseButton.enabled = true;
        purchaseButtonText.text = "Buy";
    }

    public void PurchaseItem()
    {
        if (purchased || PlayerController.instance.GetCoins() < price)
        {
            return;
        }

        Debug.Log($"Purchased item {arcana} {(isFuture ? "Future" : "Present")} ({quantity}) for {price} coins");

        PlayerController.instance.ChangeCoins(-price, true);

        purchased = true;

        purchaseButton.enabled = false;
        purchaseButtonText.text = "(Purchased)";

        // TODO - can remove the null check after all tarot cards are implemented
        // currently return null as placeholder for unimplemented cards
        TarotCard card = GetCard();
        if (card != null)
        {
            PlayerController.instance.tarotManager.AddCard(GetCard());
        }
    }

    public TarotCard GetCard()
    {
        switch (arcana)
        {
            case TarotCard.Arcana.Fool:
                return isFuture ? new Fool_Future(quantity) : new Fool_Present(quantity);
            case TarotCard.Arcana.Magician:
                return isFuture ? new Magician_Future(quantity) : new Magician_Present(quantity);
            case TarotCard.Arcana.HighPriestess:
                return isFuture ? new HighPriestess_Future(quantity) : new HighPriestess_Present(quantity);
            case TarotCard.Arcana.Empress:
                return isFuture ? new Empress_Future(quantity) : new Empress_Present(quantity);
            case TarotCard.Arcana.Emperor:
                return isFuture ? new Emperor_Future(quantity) : new Emperor_Present(quantity);
            case TarotCard.Arcana.Hierophant:
                return isFuture ? new Hierophant_Future(quantity) : new Hierophant_Present(quantity);
            case TarotCard.Arcana.Lovers:
                return isFuture ? new Lovers_Future(quantity) : new Lovers_Present(quantity);
            case TarotCard.Arcana.Chariot:
                return isFuture ? new Chariot_Future(quantity) : new Chariot_Present(quantity);
            case TarotCard.Arcana.Strength:
                return isFuture ? new Strength_Future(quantity) : new Strength_Present(quantity);
            case TarotCard.Arcana.Hermit:
                return isFuture ? new Hermit_Future(quantity) : new Hermit_Present(quantity);
        }
        return null;
    }
}