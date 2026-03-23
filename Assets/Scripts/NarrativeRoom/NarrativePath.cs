using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NarrativePath", menuName = "Scriptable Objects/Narrative/NarrativePath")]
public class NarrativePath : ScriptableObject
{
    public List<SingleNarrativeRoom> rooms;
}