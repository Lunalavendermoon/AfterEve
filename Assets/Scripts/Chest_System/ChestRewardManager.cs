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
    TarotCard.Arcana[] cards = new TarotCard.Arcana[3];
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
                cards[i] = TarotCard.GenRandomCardData().Item1;

                // Random quantity from 1-5
                // Past/Future cards can only generate w/ quantity of 1
                quantities[i] = Random.Range(1, 6);

                if (tarotIcons != null)
                {
                    cardUI[i].sprite = tarotIcons.GetSprite(cards[i], TarotCard.TarotType.Present);
                }
            }
        }
    }

    public void OnButtonPressed(int idx)
    {
        TarotCard card = TarotCard.GetCardFromData(cards[idx], TarotCard.TarotType.Present, quantities[idx]);
        
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