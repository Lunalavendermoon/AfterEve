using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public Image image;
    public TMP_Text cardNameText;

    public TarotIcon icons;

    TarotCard card;

    InventoryUIScript uiScript;

    public void InitItem(TarotCard card, InventoryUIScript script, bool setSidebar)
    {
        this.card = card;
        uiScript = script;

        Sprite sprite = icons.GetSprite(card);
        image.sprite = sprite;
        cardNameText.text = System.Text.RegularExpressions.Regex.Replace(
            card.arcana.ToString(), "([a-z])([A-Z])", "$1 $2").ToUpperInvariant();

        if (setSidebar)
        {
            uiScript.SetSidebar(icons.GetSprite(card), card);
        }
    }

    public void OnClicked()
    {
        uiScript.SetSidebar(icons.GetSprite(card), card);
    }
}
