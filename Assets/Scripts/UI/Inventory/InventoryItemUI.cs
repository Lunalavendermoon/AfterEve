using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public Image image;

    public TarotSpriteSO presentCards;
    public TarotSpriteSO futureCards;

    TarotCard card;

    InventoryUIScript uiScript;

    public void InitItem(TarotCard card, InventoryUIScript script, bool setSidebar)
    {
        this.card = card;
        uiScript = script;

        Sprite sprite = GetSprite();

        if (sprite != null)
        {
            image.sprite = sprite;
        }
        else
        {
            // TODO for debug only
            image.sprite = presentCards.fool;
            Debug.LogWarning($"No sprite for tarot card {card}");
        }

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
        TarotSpriteSO spriteSet;
        if (card is Present_TarotCard)
        {
            spriteSet = presentCards;
        }
        else if (card is Future_TarotCard)
        {
            spriteSet = futureCards;
        }
        else
        {
            // TODO add past cards
            return null;
        }

        switch (card.arcana)
        {
            case TarotCard.Arcana.Fool:
                return spriteSet.fool;
            case TarotCard.Arcana.Magician:
                return spriteSet.magician;
            case TarotCard.Arcana.HighPriestess:
                return spriteSet.highPriestess;
            case TarotCard.Arcana.Empress:
                return spriteSet.empress;
            case TarotCard.Arcana.Emperor:
                return spriteSet.emperor;
            case TarotCard.Arcana.Hierophant:
                return spriteSet.hierophant;
            case TarotCard.Arcana.Lovers:
                return spriteSet.lovers;
            case TarotCard.Arcana.Chariot:
                return spriteSet.chariot;
            case TarotCard.Arcana.Strength:
                return spriteSet.strength;
            case TarotCard.Arcana.Hermit:
                return spriteSet.hermit;
            case TarotCard.Arcana.WheelOfFortune:
                return spriteSet.wheelOfFortune;
            case TarotCard.Arcana.Justice:
                return spriteSet.justice;
            case TarotCard.Arcana.HangedMan:
                return spriteSet.hangedMan;
            case TarotCard.Arcana.Death:
                return spriteSet.death;
            case TarotCard.Arcana.Temperance:
                return spriteSet.temperance;
            case TarotCard.Arcana.Devil:
                return spriteSet.devil;
            case TarotCard.Arcana.Tower:
                return spriteSet.tower;
            case TarotCard.Arcana.Star:
                return spriteSet.star;
            case TarotCard.Arcana.Moon:
                return spriteSet.moon;
            case TarotCard.Arcana.Sun:
                return spriteSet.sun;
            case TarotCard.Arcana.Judgement:
                return spriteSet.judgement;
            case TarotCard.Arcana.World:
                return spriteSet.world;
        }
        return null;
    }
}