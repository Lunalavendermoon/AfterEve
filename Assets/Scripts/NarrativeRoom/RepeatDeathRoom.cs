using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RepeatDeathRoom", menuName = "Scriptable Objects/Narrative/RepeatDeathRoom")]
public class RepeatDeathRoom : ScriptableObject
{
    public enum DeathCauses
    {
        BasicEnemy,
        EliteEnemy,
        BossEnemy,
        Fallback,
        ScriptedDeath
    }
    public List<DeathCauses> conditions;
    public string dialogueNode;

    public bool CanTrigger()
    {
        return conditions.Contains(StaticGameManager.latestDeathCause);
    }
}
