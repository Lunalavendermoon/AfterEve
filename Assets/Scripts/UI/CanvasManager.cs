using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public Canvas UITestCanvas;
    public Canvas effectTestCanvas;
    public Canvas tarotTestCanvas;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (UITestCanvas.enabled)
            {
                UITestCanvas.enabled = false;
            } else
            {
                UITestCanvas.enabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            if (effectTestCanvas.enabled)
            {
                effectTestCanvas.enabled = false;
            }
            else
            {
                effectTestCanvas.enabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            if (tarotTestCanvas.enabled)
            {
                tarotTestCanvas.enabled = false;
            }
            else
            {
                tarotTestCanvas.enabled = true;
            }
        }
    }
}
