using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIScript : MonoBehaviour
{
    public GameObject canvas;

    public GameObject content;

    public ScrollRect scrollRect;

    public GameObject itemPrefab;

    // 0 = past, 1 = present, 2 = future
    int state;

    void Start()
    {
        SetInventoryDisplay(false);

        state = 1;
    }

    public void SetInventoryDisplay(bool enabled)
    {
        canvas.SetActive(enabled);

        if (enabled)
        {
            // TODO maybe also pause gameplay/player controls
            RefreshInventoryDisplay();
        }
    }

    public void SwitchDisplay(int newState)
    {
        if (newState != state)
        {
            state = newState;
            
            RefreshInventoryDisplay();
        }
    }

    void ScrollToTop()
    {
        Canvas.ForceUpdateCanvases(); // ensures layout updated
        scrollRect.verticalNormalizedPosition = 1f;
    }

    void RefreshInventoryDisplay()
    {
        for (int i = content.transform.childCount - 1; i >= 0; i--)
        {
            // Get the child GameObject using GetChild(i)
            GameObject child = content.transform.GetChild(i).gameObject;
            // Destroy the child GameObject
            Destroy(child);
        }

        if (state == 0)
        {
            foreach (Past_TarotCard card in TarotManager.instance.pastTarot.Values)
            {
                InstantiateTarotItem(card);
            }
        }
        else if (state == 1)
        {
            foreach (Present_TarotCard card in TarotManager.instance.presentTarot.Values)
            {
                InstantiateTarotItem(card);
            }
        }
        else
        {
            foreach (Future_TarotCard card in TarotManager.instance.futureTarot)
            {
                InstantiateTarotItem(card);
            }
        }

        ScrollToTop();
    }

    void InstantiateTarotItem(TarotCard card)
    {
        GameObject go = Instantiate(itemPrefab, content.transform);
        go.GetComponent<InventoryItemUI>().InitItem(card);
    }
}