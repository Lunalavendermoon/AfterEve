using System;
using UnityEngine;

public class ShopConfirmMenu : MonoBehaviour
{
    Action purchaseItemAction;

    public void InitializeConfirmMenu(Action purchaseItemAction)
    {
        gameObject.SetActive(true);
        this.purchaseItemAction = purchaseItemAction;
        // TODO set tarot sprite in UI
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