using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUIScript : MonoBehaviour
{
    public GameObject canvas;

    public GameObject content;

    public ScrollRect scrollRect;

    public GameObject itemPrefab;

    public Image sidebarImage;

    // 0 = past, 1 = present, 2 = future
    int state;

    void OnEnable()
    {
        PlayerController.instance.playerInput.Player.ToggleInventory.performed += OnInventoryToggled;
        PlayerController.instance.playerInput.Player.ExitInventory.performed += OnInventoryExited;
    }

    void OnDisable()
    {
        PlayerController.instance.playerInput.Player.ToggleInventory.performed -= OnInventoryToggled;
        PlayerController.instance.playerInput.Player.ExitInventory.performed -= OnInventoryExited;
    }

    private void OnInventoryToggled(InputAction.CallbackContext context)
    {
        SetInventoryDisplay(true);
    }

    private void OnInventoryExited(InputAction.CallbackContext context)
    {
        SetInventoryDisplay(false);
    }

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

        bool first = true;

        if (state == 0)
        {
            foreach (Past_TarotCard card in TarotManager.instance.pastTarot.Values)
            {
                InstantiateTarotItem(card, first);
                first = false;
            }
        }
        else if (state == 1)
        {
            foreach (Present_TarotCard card in TarotManager.instance.presentTarot.Values)
            {
                InstantiateTarotItem(card, first);
                first = false;
            }
        }
        else
        {
            foreach (Future_TarotCard card in TarotManager.instance.futureTarot)
            {
                InstantiateTarotItem(card, first);
                first = false;
            }
        }
        if (first)
        {
            SetEmptySidebar();
        }

        ScrollToTop();
    }

    void InstantiateTarotItem(TarotCard card, bool setSidebar)
    {
        GameObject go = Instantiate(itemPrefab, content.transform);
        go.GetComponent<InventoryItemUI>().InitItem(card, this, setSidebar);
    }

    public void SetSidebar(Sprite cardSprite)
    {
        sidebarImage.enabled = true;
        sidebarImage.sprite = cardSprite;
    }

    public void SetEmptySidebar()
    {
        sidebarImage.enabled = false;
    }
}