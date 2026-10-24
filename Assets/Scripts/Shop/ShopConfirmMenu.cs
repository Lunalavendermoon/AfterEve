using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopConfirmMenu : MonoBehaviour
{
    public TarotIcon tarotIcons;
    public TMP_Text priceText;
    public TMP_Text priceSubtractText;
    public TMP_Text priceSubtractResult;
    public Image selectedItem;

    Action purchaseItemAction;

    public void InitializeConfirmMenu(Action purchaseItemAction, int price, ShopItem.ShopItemType shopItemType,
                                        TarotCard.Arcana arcana = TarotCard.Arcana.Fool,
                                        TarotCard.TarotType tarotType = TarotCard.TarotType.Present)
    {
        gameObject.SetActive(true);
        this.purchaseItemAction = purchaseItemAction;
        priceText.text = $"{price}";
        priceSubtractText.text = $"-{price}";
        priceSubtractResult.text = $"{PlayerController.instance.GetCoins() - price}";

        if (shopItemType == ShopItem.ShopItemType.Tarot)
        {
            selectedItem.sprite = tarotIcons.GetSprite(arcana, tarotType);
        }
        else
        {
            // TODO
        }
    }

    public void HideConfirmMenu()
    {
        gameObject.SetActive(false);
    }

    public void ConfirmPurchase()
    {
        purchaseItemAction();
        gameObject.SetActive(false);
    }
}