using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public enum ShopItemType
    {
        Tarot,
        LuckyCoin,
        SkillSlot
    }
    ShopItemType shopItemType;

    public TarotIcon tarotIcons;

    TarotCard.Arcana arcana;
    TarotCard.TarotType tarotType;
    int price;
    int quantity;

    Dictionary<TarotCard.TarotType, bool> purchased = new();

    public Image itemSprite;
    public TMP_Text arcanaText;
    public Button purchaseButton;
    public TMP_Text purchaseButtonText;

    TarotCard dummyTarotForDescription;

    public void InitializeShopItem(TarotCard.Arcana arcana, TarotCard.TarotType tarotType, int price, int quantity)
    {
        shopItemType = ShopItemType.Tarot;
        this.arcana = arcana;
        this.tarotType = tarotType;
        this.price = price;
        this.quantity = quantity;

        arcanaText.text = TarotCard.GetLocalizedArcanaName(arcana);
        dummyTarotForDescription = TarotCard.GetCardFromData(arcana, tarotType);

        // Cannot purchast Past or Present cards beyond stack limit
        purchased[TarotCard.TarotType.Past] = TarotManager.instance.pastTarot.ContainsKey(arcana);

        purchased[TarotCard.TarotType.Present] = TarotManager.instance.presentTarot.ContainsKey(arcana) &&
            TarotManager.instance.presentTarot[arcana].level >= 4; // zero-indexed level 5

        purchased[TarotCard.TarotType.Future] = false;

        ResetDisplay();
    }

    public void InitializeShopItem(ShopItemType shopItemType, int price)
    {
        this.shopItemType = shopItemType;
        this.price = price;

        arcanaText.text = "";

        // Cannot purchast Past or Present cards beyond stack limit
        purchased[TarotCard.TarotType.Past] = TarotManager.instance.pastTarot.ContainsKey(arcana);

        purchased[TarotCard.TarotType.Present] = TarotManager.instance.presentTarot.ContainsKey(arcana) &&
            TarotManager.instance.presentTarot[arcana].level >= 4; // zero-indexed level 5

        purchased[TarotCard.TarotType.Future] = false;

        ResetDisplay();
    }

    void ResetDisplay()
    {
        if (shopItemType == ShopItemType.Tarot)
        {
            itemSprite.sprite = tarotIcons.GetSprite(arcana, tarotType);
        }
        else
        {
            // TODO
        }
        purchaseButtonText.text = $"{price}";
        SetButtonAvailable();
    }

    public void OnClicked()
    {
        ShopManager.instance.InitializeConfirmMenu(() => ConfirmPurchaseItem(), price,
            tarotIcons.GetSprite(arcana, tarotType),
            TarotCard.GetLocalizedArcanaName(arcana),
            dummyTarotForDescription.GetDescription());
    }

    void ConfirmPurchaseItem()
    {
        if (purchased[tarotType] || PlayerController.instance.GetCoins() < price)
        {
            return;
        }

        PlayerController.instance.ChangeCoins(-price, true);
        ShopManager.instance.RefreshCurrentCoinsText();

        purchased[tarotType] = true;

        purchaseButton.enabled = false;
        purchaseButtonText.text = "--";

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
            purchaseButtonText.text = "--";
            return;
        }
        if (tarotType == TarotCard.TarotType.Past && StaticGameManager.pastCards.Contains(arcana))
        {
            // past cards cannot stack, disable purchase button if player already has this arcana
            purchaseButton.enabled = false;
            purchaseButtonText.text = "--";
            return;
        }
        purchaseButton.enabled = PlayerController.instance.GetCoins() >= price;
        purchaseButtonText.text = $"{price}";
    }
}