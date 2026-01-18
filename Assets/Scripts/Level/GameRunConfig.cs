using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Run Config")]
public class GameRunConfig : ScriptableObject
{
    [Tooltip("Ordered sequence of room kinds for this run")]
    public List<GameRoomKind> roomSequence;
}
