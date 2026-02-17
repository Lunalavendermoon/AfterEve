using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public Image image;

    public TarotSpriteSO presentCards;
    public TarotSpriteSO futureCards;

    TarotCard card;

    public void InitItem(TarotCard card)
    {
        this.card = card;
        Sprite sprite;
        if (card is Present_TarotCard)
        {
            sprite = GetSprite(presentCards);
        }
        else if (card is Future_TarotCard)
        {
            sprite = GetSprite(futureCards);
        }
        else
        {
            // TODO add past cards
            sprite = null;
        }

        if (sprite != null)
        {
            image.sprite = sprite;
        }
        else
        {
            // TODO for debug only
            Debug.LogWarning($"No sprite for tarot card {card}");
        }
    }

    Sprite GetSprite(TarotSpriteSO spriteSet)
    {
        switch (card.arcana)
        {
            case TarotCard.Arcana.Fool:
                return spriteSet.fool;
            case TarotCard.Arcana.Magician:
                return spriteSet.magician;
        }
        return null;
    }
}