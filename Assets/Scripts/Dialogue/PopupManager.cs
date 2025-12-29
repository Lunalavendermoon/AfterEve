using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PopupEvent // one enum for each event
{
    EnemyDefeated,
    ChestOpened,
    IdleTooLong,
    Test,
}

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private List<PopupDialogueSO> allPopups;

    private PopupUIController popupUI;
    private Dictionary<PopupEvent, PopupDialogueSO> popupLookup;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        popupUI = FindAnyObjectByType<PopupUIController>();
        BuildLookup();
    }

    private void BuildLookup()
    {
        popupLookup = new Dictionary<PopupEvent, PopupDialogueSO>();

        foreach (PopupDialogueSO popup in allPopups)
        {
            if (popup == null) continue;

            if (popupLookup.ContainsKey(popup.GetTriggerEvent()))
            {
                Debug.LogError("Duplicate popup key " + popup.GetTriggerEvent() + " cannot be added to the lookup");
                continue;
            }
            popupLookup.Add(popup.GetTriggerEvent(), popup);
        }
    }

    public void TriggerPopupEvent(PopupEvent popupEvent)
    {
        if (popupUI.isActive) return; //ignore other events while popup is active
        if (!popupLookup.TryGetValue(popupEvent, out var popup)) return;
        if (popup.IsOnCooldown()) return;

        DisplayPopup(popup);
    }

    private void DisplayPopup(PopupDialogueSO popup)
    {
        string line = popup.GetRandomLine();
        if (string.IsNullOrEmpty(line)) return;

        popup.SaveLastPlayedTime();
        popupUI.Show(line);
    }

    private void Update()
    {
        // For testing purposes
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Testing popup event trigger");
            TriggerPopupEvent(PopupEvent.Test);
        }
    }
}
