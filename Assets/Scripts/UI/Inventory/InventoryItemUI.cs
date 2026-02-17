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

        Sprite sprite = GetSprite();
        image.sprite = sprite;

        if (setSidebar)
        {
            uiScript.SetSidebar(GetSprite());
        }
    }

    public void OnClicked()
    {
        uiScript.SetSidebar(GetSprite());
    }

    Sprite GetSprite()
    {
        return icons.GetSprite(card);
    }
}