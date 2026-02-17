using UnityEngine;

public class InventoryUIScript : MonoBehaviour
{
    public GameObject canvas;

    public InventoryContentLayout layout;

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

    void RefreshInventoryDisplay()
    {
        Debug.Log($"Set card display to state: {state}");
    }
}