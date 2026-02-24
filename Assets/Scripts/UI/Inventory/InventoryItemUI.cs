using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public Image image;

    public TarotIcon icons;

    TarotCard card;

    InventoryUIScript uiScript;

    public void InitItem(TarotCard card, InventoryUIScript script, bool setSidebar)
    {
        this.card = card;
        uiScript = script;

        Sprite sprite = icons.GetSprite(card);
        image.sprite = sprite;

        if (setSidebar)
        {
            uiScript.SetSidebar(icons.GetSprite(card), card.GetDescription());
        }
    }

    public void OnClicked()
    {
        uiScript.SetSidebar(icons.GetSprite(card), card.GetDescription());
    }
}