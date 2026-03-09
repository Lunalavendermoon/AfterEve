using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeathRooms", menuName = "Scriptable Objects/Narrative/DeathRooms")]
public class DeathRooms : ScriptableObject
{
    public List<SingleTimeDeathRoom> singleTime;

    public List<RepeatDeathRoom> repeats;
}