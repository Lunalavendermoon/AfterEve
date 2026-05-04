using UnityEngine;
using UnityEngine.UI;

public class ScaleGridCell : MonoBehaviour
{
    [SerializeField] private float cardAspectRatio = 1f;
    [SerializeField] private bool scaleVertically = false;

    private GridLayoutGroup grid;
    private RectTransform rectTransform;

    private void Start()
    {
        this.grid = this.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            Debug.LogError("GridLayoutGroup component not found on " + this.name + ". Please add it to the GameObject.");
            return;
        }

        this.rectTransform = this.GetComponentInParent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component not found on the parent of " + this.name + ". Please add it to the GameObject.");
            return;
        }

        UpdateCellSize();
    }

    // gets triggered when there is a change in parent panel size or screen size
    private void OnRectTransformDimensionsChange() => UpdateCellSize();

    private void UpdateCellSize()
    {
        if (grid == null || rectTransform == null) return;

        int maxCount = this.grid.constraintCount;
        float spacing = 0;
        float padding = 0;

        float newWidth = this.rectTransform.rect.width;
        float newHeight = this.rectTransform.rect.height;

        if (scaleVertically)
        {
            spacing = this.grid.spacing.y;
            padding = this.grid.padding.bottom + this.grid.padding.top;
            newHeight = (newHeight - (spacing * (maxCount - 1)) - padding) / maxCount;
            if (cardAspectRatio != 0) newWidth = newHeight / cardAspectRatio;
        }
        else
        {
            spacing = this.grid.spacing.x;
            padding = this.grid.padding.left + this.grid.padding.right;

            newWidth = (newWidth - (spacing * (maxCount - 1)) - padding) / maxCount;
            if (cardAspectRatio != 0) newHeight = newWidth / cardAspectRatio;
        }

        // Debug.Log(newWidth);
        this.grid.cellSize = new Vector2(newWidth, newHeight);
    }
}
