using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitManager : MonoBehaviour
{
    public List<PortraitEntry> portraits = new List<PortraitEntry>();
    public static PortraitManager instance;
    public Image portraitContainer;
    public Image cgContainer;

    private Dictionary<string, Dictionary<string, Sprite>> portraitLookup;

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

    public void SetCG(Sprite cg)
    {
        StartCoroutine(SetCGCoroutine(cg));
    }

    private IEnumerator SetCGCoroutine(Sprite cg)
    {
        yield return null; // ensures main thread + next frame

        cgContainer.sprite = cg;
        cgContainer.color = Color.white;
    }

    public void ClearCG()
    {
        StartCoroutine(ClearCGCoroutine());
    }

    private IEnumerator ClearCGCoroutine()
    {
        yield return null; // ensures main thread + next frame

        cgContainer.sprite = null;
        cgContainer.color = new Color(1, 1, 1, 0);
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
