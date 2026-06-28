using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicatorManager : MonoBehaviour
{
    public static EnemyIndicatorManager Instance;

    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Color arrowColor = Color.red;
    [SerializeField] private float arrowSize = 40f;
    [SerializeField] private float screenPadding = 60f;

    private Canvas canvas;
    private RectTransform canvasRect;
    private Camera cam;
    private readonly Dictionary<EnemyBase, RectTransform> indicators = new Dictionary<EnemyBase, RectTransform>();
    private Sprite defaultArrowSprite;

    void Awake()
    {
        Instance = this;
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        cam = Camera.main;

        if (arrowSprite == null)
            arrowSprite = CreateDefaultArrowSprite();
    }

    void Start()
    {
        // Pick up enemies already active in the scene
        foreach (EnemyBase enemy in FindObjectsOfType<EnemyBase>())
            AddIndicator(enemy);
    }

    void OnEnable()
    {
        EnemyBase.OnEnemySpawned += AddIndicator;
        EnemyBase.OnEnemyDestroyed += RemoveIndicator;
        EnemyBase.OnEnemyDeath += OnEnemyDied;
    }

    void OnDisable()
    {
        EnemyBase.OnEnemySpawned -= AddIndicator;
        EnemyBase.OnEnemyDestroyed -= RemoveIndicator;
        EnemyBase.OnEnemyDeath -= OnEnemyDied;
    }

    private void AddIndicator(EnemyBase enemy)
    {
        if (indicators.ContainsKey(enemy)) return;
        indicators[enemy] = CreateArrow();
    }

    private void RemoveIndicator(EnemyBase enemy)
    {
        if (!indicators.TryGetValue(enemy, out RectTransform rt)) return;
        if (rt != null) Destroy(rt.gameObject);
        indicators.Remove(enemy);
    }

    private void OnEnemyDied(DamageInstance dmg, EnemyBase enemy) => RemoveIndicator(enemy);

    private RectTransform CreateArrow()
    {
        GameObject go = new GameObject("EnemyArrow", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = Vector2.one * arrowSize;

        Image img = go.GetComponent<Image>();
        img.sprite = arrowSprite;
        img.color = arrowColor;
        img.raycastTarget = false;

        return rt;
    }

    void LateUpdate()
    {
        foreach (KeyValuePair<EnemyBase, RectTransform> kvp in indicators)
        {
            if (kvp.Key == null || kvp.Value == null) continue;
            UpdateArrow(kvp.Value, kvp.Key.transform.position);
        }
    }

    private void UpdateArrow(RectTransform arrow, Vector3 worldPos)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        // Behind camera — flip so direction still makes sense
        if (screenPos.z < 0)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

        float pad = screenPadding;
        bool onScreen = screenPos.z > 0
            && screenPos.x > pad && screenPos.x < Screen.width - pad
            && screenPos.y > pad && screenPos.y < Screen.height - pad;

        arrow.gameObject.SetActive(!onScreen);
        if (onScreen) return;

        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 dir = ((Vector2)screenPos - center).normalized;

        // Rotate so the arrow sprite (pointing up) faces toward the enemy
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // Clamp to screen edge rectangle
        float halfW = center.x - pad;
        float halfH = center.y - pad;
        float sx = (dir.x != 0f) ? halfW / Mathf.Abs(dir.x) : float.MaxValue;
        float sy = (dir.y != 0f) ? halfH / Mathf.Abs(dir.y) : float.MaxValue;
        Vector2 edgeScreen = center + dir * Mathf.Min(sx, sy);

        // Convert to canvas-local coordinates (handles CanvasScaler automatically)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, edgeScreen, null, out Vector2 localPos);
        arrow.anchoredPosition = localPos;
    }

    // Generates a simple upward-pointing arrow texture at runtime
    private Sprite CreateDefaultArrowSprite()
    {
        int s = 64;
        Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
        Color clear = Color.clear;
        Color white = Color.white;

        for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
                tex.SetPixel(x, y, clear);

        int cx = s / 2;
        // Arrowhead: filled triangle from mid to top
        for (int y = s / 2; y < s; y++)
        {
            int half = s - 1 - y;
            for (int x = cx - half; x <= cx + half; x++)
                if (x >= 0 && x < s) tex.SetPixel(x, y, white);
        }
        // Tail: narrow rectangle
        int tw = s / 8;
        for (int y = 0; y < s / 2; y++)
            for (int x = cx - tw; x <= cx + tw; x++)
                if (x >= 0 && x < s) tex.SetPixel(x, y, white);

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f));
    }
}