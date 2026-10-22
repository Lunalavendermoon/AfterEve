using System;
using System.Collections.Generic;
using TMPro;
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
    public TMP_Text sidebarTitle;
    public TMP_Text sidebarDesc;

    // 0 = past, 1 = present, 2 = future
    int state;

    public bool IsOpen => canvas != null && canvas.activeInHierarchy;
    public static int LastClosedFrame { get; private set; } = -1;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    void TraceInventory(string action)
    {
        TarotManager manager = TarotManager.instance;
        Debug.Log($"[Inventory] {action}; frame={Time.frameCount}, tab={state}, open={IsOpen}, " +
            $"timeScale={Time.timeScale}, manager={(manager != null ? manager.GetInstanceID() : 0)}, " +
            $"past={manager?.pastTarot.Count}, present={manager?.presentTarot.Count}, future={manager?.futureTarot.Count}", this);
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!IsOpen || Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        var eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem == null)
        {
            TraceInventory("Pointer click without an EventSystem");
            return;
        }

        var pointer = new UnityEngine.EventSystems.PointerEventData(eventSystem)
        {
            position = Mouse.current.position.ReadValue()
        };
        var hits = new List<UnityEngine.EventSystems.RaycastResult>();
        eventSystem.RaycastAll(pointer, hits);
        var targets = new List<string>();
        foreach (var hit in hits)
        {
            string path = hit.gameObject.name;
            for (Transform parent = hit.gameObject.transform.parent; parent != null; parent = parent.parent)
                path = parent.name + "/" + path;
            targets.Add(path);
        }
        TraceInventory($"Pointer raycast hits (front first): {string.Join(", ", targets)}");
    }
#endif

    // void OnEnable()
    // {
    //     PlayerController.instance.playerInput.Player.ToggleInventory.performed += OnInventoryToggled;
    //     PlayerController.instance.playerInput.Player.ExitInventory.performed += OnInventoryExited;
    // }

    // void OnDisable()
    // {
    //     PlayerController.instance.playerInput.Player.ToggleInventory.performed -= OnInventoryToggled;
    //     PlayerController.instance.playerInput.Player.ExitInventory.performed -= OnInventoryExited;
    // }

    private void OnInventoryToggled(InputAction.CallbackContext context)
    {
        SetInventoryDisplay(true);
    }

    private void OnInventoryExited(InputAction.CallbackContext context)
    {
        if (IsOpen)
            SetInventoryDisplay(false);
    }

    void Start()
    {
        SetInventoryDisplay(false);

        state = 1;

        PlayerController.instance.playerInput.Player.ToggleInventory.performed += OnInventoryToggled;
        PlayerController.instance.playerInput.Player.ExitInventory.performed += OnInventoryExited;
    }

    public void SetInventoryDisplay(bool enabled)
    {
        TraceInventory(enabled ? "Opening" : "Closing");
        if (!enabled && IsOpen)
            LastClosedFrame = Time.frameCount;
        canvas.SetActive(enabled);

        if (enabled)
        {
            // TODO maybe also pause gameplay/player controls
            RefreshInventoryDisplay();
        }
    }

    public void SwitchDisplay(int newState)
    {
        TraceInventory($"Tab requested={newState}, previous={state}");
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
        TraceInventory($"Refresh started; children={content.transform.childCount}");
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
        TraceInventory($"Refresh completed; empty={first}, children={content.transform.childCount}, " +
            $"contentActive={content.activeInHierarchy}, sidebarActive={sidebarImage.gameObject.activeInHierarchy}");
    }

    void InstantiateTarotItem(TarotCard card, bool setSidebar)
    {
        TraceInventory($"Initializing {card.arcana}, type={card.tarotType}, sidebar={setSidebar}");
        GameObject go = Instantiate(itemPrefab, content.transform);
        go.GetComponent<InventoryItemUI>().InitItem(card, this, setSidebar);
        TraceInventory($"Initialized {card.arcana}; itemActive={go.activeInHierarchy}");
    }

    public void SetSidebar(Sprite cardSprite, TarotCard card)
    {
        sidebarImage.enabled = true;
        sidebarImage.sprite = cardSprite;
        sidebarTitle.text = System.Text.RegularExpressions.Regex.Replace(
            card.arcana.ToString(), "([a-z])([A-Z])", "$1 $2").ToUpperInvariant();
        sidebarDesc.text = card.GetDescription();
        TraceInventory($"Sidebar populated: {card.arcana}");
    }

    public void SetEmptySidebar()
    {
        sidebarImage.enabled = false;
        sidebarTitle.text = "";
        sidebarDesc.text = "";
    }
}
