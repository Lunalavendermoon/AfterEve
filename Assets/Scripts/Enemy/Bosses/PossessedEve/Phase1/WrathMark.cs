using System.Collections;
using UnityEngine;

public class WrathMark : MonoBehaviour
{
    [SerializeField] float spiritualEarlySeconds = 0.75f;
    [SerializeField] float everyoneSeconds = 0.5f;
    [SerializeField] SpriteRenderer sprite;
    public bool Done { get; private set; }
    public void Begin(System.Action onComplete)
    {
        StartCoroutine(Run(onComplete));
    }
    IEnumerator Run(System.Action onComplete)
    {
        float t = 0f;
        while (t < spiritualEarlySeconds)
        {
            t += Time.deltaTime;
            bool spirit = PlayerController.instance != null && PlayerController.instance.IsInSpiritualVision();
            SetAlpha(spirit ? 0.7f : 0.2f);
            yield return null;
        }
        t = 0f;
        while (t < everyoneSeconds)
        {
            t += Time.deltaTime;
            SetAlpha(1f);
            yield return null;
        }
        Done = true;
        onComplete?.Invoke();
    }
    void SetAlpha(float a)
    {
        if (sprite == null) return;
        var c = sprite.color;
        c.a = a;
        sprite.color = c;
    }
}
