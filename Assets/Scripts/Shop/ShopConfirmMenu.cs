using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopConfirmMenu : MonoBehaviour
{
    public TMP_Text arcanaText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    public TMP_Text priceSubtractText;
    public TMP_Text priceSubtractResult;
    public Image selectedItem;

    Action purchaseItemAction;

    public void InitializeConfirmMenu(Action purchaseItemAction, int price, Sprite itemSprite, string itemTitle, string itemDescription)
    {
        gameObject.SetActive(true);
        this.purchaseItemAction = purchaseItemAction;
        priceText.text = $"{price}";
        priceSubtractText.text = $"-{price}";
        priceSubtractResult.text = $"{PlayerController.instance.GetCoins() - price}";

        if (itemSprite != null)
        {
            // TODO remove null check once all sprites are imported
            selectedItem.sprite = itemSprite;
        }
        arcanaText.text = itemTitle;
        descriptionText.text = itemDescription;
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