using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitManager : MonoBehaviour
{
    public List<PortraitEntry> portraits = new List<PortraitEntry>();
    public static PortraitManager instance;

    private Image portraitContainer;
    private Dictionary<string, Dictionary<string, Sprite>> portraitLookup;

    private void Awake()
    {
        if (instance == null) instance = this;

        portraitContainer = GetComponent<Image>();
        if (portraitContainer == null)
        {
            Debug.LogError("PortraitManager requires an Image component on the same GameObject.");
        }   

        BuildLookup();
    }

    private void BuildLookup()
    {
        portraitLookup = new Dictionary<string, Dictionary<string, Sprite>>();

        foreach (PortraitEntry character in portraits)
        {
            if (string.IsNullOrEmpty(character.characterName) || portraitLookup.ContainsKey(character.characterName)) continue;

            Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

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
