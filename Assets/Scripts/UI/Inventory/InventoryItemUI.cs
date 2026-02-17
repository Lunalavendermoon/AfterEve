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
        TarotIcon.TarotType type;
        if (card is Past_TarotCard)
        {
            type = TarotIcon.TarotType.Past;
        }
        else if (card is Present_TarotCard)
        {
            type = TarotIcon.TarotType.Present;
        }
        else
        {
            type = TarotIcon.TarotType.Future;
        }
        return icons.GetSprite(card.arcana, type);
    }
}