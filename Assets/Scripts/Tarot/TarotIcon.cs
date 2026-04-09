using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TarotIcon", menuName = "Scriptable Objects/TarotIcon")]
public class TarotIcon : ScriptableObject
{
    [SerializeField] private Sprite[] fool;
    [SerializeField] private Sprite[] magician;
    [SerializeField] private Sprite[] highPriestess;
    [SerializeField] private Sprite[] empress;
    [SerializeField] private Sprite[] emperor;
    [SerializeField] private Sprite[] hierophant;
    [SerializeField] private Sprite[] lovers;
    [SerializeField] private Sprite[] chariot;
    [SerializeField] private Sprite[] strength;
    [SerializeField] private Sprite[] hermit;
    [SerializeField] private Sprite[] wheelOfFortune;

    public Sprite GetFutureSkillSprite(TarotCard.Arcana arcana)
    {
        return GetSprite(arcana, TarotCard.TarotType.Future);
    }

    public Sprite GetSprite(TarotCard card)
    {
        if(card is Present_TarotCard) return GetSprite(card.arcana, TarotCard.TarotType.Present);
        if(card is Future_TarotCard) return GetSprite(card.arcana, TarotCard.TarotType.Future);
        return GetSprite(card.arcana, TarotCard.TarotType.Present); // since past sprites do not exist
    }

    public Sprite GetSprite (TarotCard.Arcana arcana, TarotCard.TarotType type)
    {
        // for now since past sprites do not exist
        if(type == TarotCard.TarotType.Past) type = TarotCard.TarotType.Present;

        switch(arcana)
        {
            case TarotCard.Arcana.Fool:
                return fool[(int)type];
            case TarotCard.Arcana.Magician:
                return magician[(int)type];
            case TarotCard.Arcana.HighPriestess:
                return highPriestess[(int)type];
            case TarotCard.Arcana.Empress:
                return empress[(int)type];
            case TarotCard.Arcana.Emperor:
                return emperor[(int)type];
            case TarotCard.Arcana.Hierophant:
                return hierophant[(int)type];
            case TarotCard.Arcana.Lovers:
                return lovers[(int)type];
            case TarotCard.Arcana.Chariot:
                return chariot[(int)type];
            case TarotCard.Arcana.Strength:
                return strength[(int)type];
            case TarotCard.Arcana.Hermit:
                return hermit[(int)type];
            case TarotCard.Arcana.WheelOfFortune:
                return wheelOfFortune[(int)type];
        }

        return fool[1]; // fallback
    }
}
