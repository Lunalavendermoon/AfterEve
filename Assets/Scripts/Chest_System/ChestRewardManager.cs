using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestRewardManager : MonoBehaviour
{
    public static ChestRewardManager instance;
    public GameObject chestRewardCanvas;

    public TarotIcon tarotIcons;
    public List<Image> cardUI;

    public List<GameObject> tarotButtons;
    public List<TMP_Text> texts;
    (TarotCard.Arcana, bool)[] cards = new (TarotCard.Arcana, bool)[3];
    int[] quantities = new int[3];

    private Vector3? lastChestWorldPos;

    void Start()
    {
        if (instance == null) instance = this;

        chestRewardCanvas.SetActive(false);
    }

    public void SetLastChestWorldPos(Vector3 pos)
    {
        lastChestWorldPos = pos;
    }

    public void ShowChestRewardMenu(bool refresh = true)
    {
        chestRewardCanvas.SetActive(true);
        PlayerController.instance.DisablePlayerInput();

        if (refresh)
        {
            for (int i = 0; i < 3; ++i)
            {
                cards[i] = TarotCard.GenRandomCardData();

                // Random quantity from 1-5
                // Future cards can only generate w/ quantity of 1
                quantities[i] = cards[i].Item2 ? 1 : Random.Range(1, 6);

                texts[i].text = $"{cards[i].Item1} {(cards[i].Item2 ? "Future" : "Present")} ({quantities[i]})";
                TarotIcon.TarotType type = cards[i].Item2 ? TarotIcon.TarotType.Future : TarotIcon.TarotType.Present;

                if (tarotIcons != null)
                {
                    cardUI[i].sprite = tarotIcons.GetSprite(cards[i].Item1, type);
                }
            }
        }
    }

    public void OnButtonPressed(int idx)
    {
        TarotCard card = TarotCard.GetPresentFutureCard(cards[idx].Item1, cards[idx].Item2, quantities[idx]);
        
        if (card != null)
        {
            PlayerController.instance.tarotManager.AddCard(card);
        }

        chestRewardCanvas.SetActive(false);
        PlayerController.instance.EnablePlayerInput();

        if (lastChestWorldPos.HasValue && GameManager.instance != null)
        {
            GameManager.instance.SpawnPortalNearChest(lastChestWorldPos.Value);
        }
    }
}