using UnityEngine;

[CreateAssetMenu(fileName = "TarotIcon", menuName = "Scriptable Objects/TarotIcon")]
public class TarotIcon : ScriptableObject
{
    public Sprite foolTarot;

    public Sprite GetSprite (TarotCard.Arcana arcana)
    {
        // TODO return sprite based on arcana
        return foolTarot;
    }
}
