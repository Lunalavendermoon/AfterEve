using UnityEngine;
using UnityEngine.UI;

public class ScaleLayoutElement : MonoBehaviour
{
    [SerializeField] private float layoutAspectRatio = 1f;
    [SerializeField] private bool scaleHeight = false;

    private LayoutElement layout;
    private RectTransform rectTransform;

    private void Start()
    {
        this.layout = this.GetComponent<LayoutElement>();
        if (layout == null)
        {
            Debug.LogError("LayoutElement component not found on " + this.name + ". Please add it to the GameObject.");
            return;
        }

        this.rectTransform = this.GetComponentInParent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component not found on the parent of " + this.name + ". Please add it to the GameObject.");
            return;
        }

        UpdatePreferredSize();
    }

    private void OnRectTransformDimensionsChange() => UpdatePreferredSize();

    private void UpdatePreferredSize()
    {
        if (layout == null || rectTransform == null) return;

        float newWidth = this.rectTransform.rect.width;
        float newHeight = this.rectTransform.rect.height;

        if (scaleHeight)
        {
            if (layoutAspectRatio != 0) newHeight = newWidth / layoutAspectRatio;
            this.layout.preferredHeight = newHeight;
        }
        else
        {
            if (layoutAspectRatio != 0) newWidth = newHeight / layoutAspectRatio;
            this.layout.preferredWidth = newWidth;
        }
    }
}
