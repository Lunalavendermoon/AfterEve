using System.Collections.Generic;
using UnityEngine;

public class RepeatDeathRoom : ScriptableObject
{
    public enum DeathCauses
    {
        BasicEnemy,
        EliteEnemy,
        BossEnemy,
        Fallback
    }
    public int pathCount;
    public List<DeathCauses> conditions;

    public bool CanTrigger(int path, DeathCauses cond)
    {
        return path == pathCount && conditions.Contains(cond);
    }
}