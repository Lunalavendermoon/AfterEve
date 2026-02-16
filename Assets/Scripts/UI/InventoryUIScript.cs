using UnityEngine;

public class InventoryUIScript : MonoBehaviour
{
    public GameObject canvas;

    void Start()
    {
        SetInventoryDisplay(false);
    }

    public void SetInventoryDisplay(bool enabled)
    {
        canvas.SetActive(enabled);
    }
}