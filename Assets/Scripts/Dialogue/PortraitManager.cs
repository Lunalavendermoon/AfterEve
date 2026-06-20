using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitManager : MonoBehaviour
{
    public List<PortraitEntry> portraits = new List<PortraitEntry>();
    public Sprite blackBackground;
    public static PortraitManager instance;
    public Image portraitContainer;
    public Image cgContainer;
    [SerializeField] private float cgFadeDuration = 0.5f;

    private Dictionary<string, Dictionary<string, Sprite>> portraitLookup;
    private Coroutine cgCoroutine;
    private Image cgFadeOverlay;

    private void Awake()
    {
        if (instance == null) instance = this;

        BuildLookup();
    }

    private void BuildLookup()
    {
        // Case-insensitive string lookup
        portraitLookup = new Dictionary<string, Dictionary<string, Sprite>>(StringComparer.OrdinalIgnoreCase);

        foreach (PortraitEntry character in portraits)
        {
            if (string.IsNullOrEmpty(character.characterName) || portraitLookup.ContainsKey(character.characterName)) continue;

            Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);

            foreach (PortraitSpriteEntry spriteEntry in character.sprites)
            {
                if (string.IsNullOrEmpty(spriteEntry.spriteName) || spriteDict.ContainsKey(spriteEntry.spriteName)) continue;

                spriteDict.Add(spriteEntry.spriteName, spriteEntry.sprite);
            }

            portraitLookup.Add(character.characterName, spriteDict);
        }
    }

    public void SetPortrait(string characterName, string spriteName)
    {
        StartCoroutine(SetPortraitCoroutine(characterName, spriteName));
    }

    private IEnumerator SetPortraitCoroutine(string characterName, string spriteName)
    {
        yield return null; // ensures main thread + next frame

        if (portraitLookup.TryGetValue(characterName, out var sprites) && sprites.TryGetValue(spriteName, out var sprite))
        {
            portraitContainer.sprite = sprite;
            portraitContainer.color = Color.white;
        }
        else
        {
            Debug.LogWarning($"Portrait not found: {characterName} - {spriteName}");
        }
    }

    public void ClearPortrait()
    {
        StartCoroutine(ClearPortraitCoroutine());
    }

    private IEnumerator ClearPortraitCoroutine()
    {
        yield return null; // ensures main thread + next frame

        portraitContainer.sprite = null;
        portraitContainer.color = new Color(1, 1, 1, 0);
    }

    public void SetCG(Sprite cg, bool fade)
    {
        StopCGCoroutine();
        cgCoroutine = StartCoroutine(SetCGCoroutine(cg, fade));
    }

    public void SetBlackCG(bool fade)
    {
        StopCGCoroutine();
        cgCoroutine = StartCoroutine(SetCGCoroutine(blackBackground, fade));
    }

    private IEnumerator SetCGCoroutine(Sprite cg, bool fade)
    {
        yield return null; // ensures main thread + next frame

        if (!fade)
        {
            cgContainer.sprite = cg;
            cgContainer.color = Color.white;
            cgCoroutine = null;
            yield break;
        }

        Image previousCG = null;
        float newStartAlpha = cgContainer.color.a;
        bool hasPreviousCG = cgContainer.sprite != null && cgContainer.color.a > 0f;

        if (hasPreviousCG)
        {
            previousCG = Instantiate(cgContainer, cgContainer.transform.parent);
            cgFadeOverlay = previousCG;
            newStartAlpha = 0f;
            previousCG.transform.SetSiblingIndex(cgContainer.transform.GetSiblingIndex());
            previousCG.raycastTarget = false;
        }

        cgContainer.sprite = cg;
        cgContainer.color = new Color(1, 1, 1, newStartAlpha);

        float elapsedTime = 0f;

        while (elapsedTime < cgFadeDuration)
        {
            float fadeProgress = cgFadeDuration <= 0f ? 1f : elapsedTime / cgFadeDuration;

            cgContainer.color = new Color(1, 1, 1, Mathf.Lerp(newStartAlpha, 1f, fadeProgress));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cgContainer.color = Color.white;

        if (previousCG != null)
        {
            Destroy(previousCG.gameObject);
            cgFadeOverlay = null;
        }

        cgCoroutine = null;
    }

    public void ClearCG()
    {
        StopCGCoroutine();
        cgCoroutine = StartCoroutine(ClearCGCoroutine());
    }

    private IEnumerator ClearCGCoroutine()
    {
        yield return null; // ensures main thread + next frame

        if (cgContainer.sprite == null || cgContainer.color.a <= 0f || cgFadeDuration <= 0f)
        {
            cgContainer.sprite = null;
            cgContainer.color = new Color(1, 1, 1, 0);
            cgCoroutine = null;
            yield break;
        }

        Image fadingCG = Instantiate(cgContainer, cgContainer.transform.parent);
        cgFadeOverlay = fadingCG;
        fadingCG.transform.SetSiblingIndex(cgContainer.transform.GetSiblingIndex() + 1);
        fadingCG.raycastTarget = false;

        Color startColor = fadingCG.color;
        cgContainer.sprite = null;
        cgContainer.color = new Color(1, 1, 1, 0);

        float elapsedTime = 0f;

        while (elapsedTime < cgFadeDuration)
        {
            float fadeProgress = elapsedTime / cgFadeDuration;

            fadingCG.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                Mathf.Lerp(startColor.a, 0f, fadeProgress));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(fadingCG.gameObject);
        cgFadeOverlay = null;
        cgCoroutine = null;
    }

    private void StopCGCoroutine()
    {
        if (cgCoroutine != null)
        {
            StopCoroutine(cgCoroutine);
            cgCoroutine = null;
        }

        if (cgFadeOverlay != null)
        {
            Destroy(cgFadeOverlay.gameObject);
            cgFadeOverlay = null;
        }
    }
}

[System.Serializable]
public class PortraitEntry
{
    public string characterName;
    public List<PortraitSpriteEntry> sprites = new List<PortraitSpriteEntry>();
}

[System.Serializable]
public class PortraitSpriteEntry
{
    public string spriteName; // e.g. "happy", "angry"
    public Sprite sprite;
}
