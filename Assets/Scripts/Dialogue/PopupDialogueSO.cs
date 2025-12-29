using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue/Popup")]
public class PopupDialogueSO : ScriptableObject
{
    [SerializeField] private PopupEvent triggerEvent;
    [SerializeField] private List<string> lines; //dialogue lines, can expand on this later
    [SerializeField] private float cooldown = 3f; // prevents popup spam

    [NonSerialized]
    private float lastPlayedTime = -Mathf.Infinity;

    public bool IsOnCooldown()
    {
        return Time.time < lastPlayedTime + cooldown;
    }

    public string GetRandomLine()
    {
        if (lines == null || lines.Count == 0) return string.Empty;

        return lines[UnityEngine.Random.Range(0, lines.Count)];
    }

    public void SaveLastPlayedTime()
    {
        lastPlayedTime = Time.time;
    }

    public PopupEvent GetTriggerEvent()
    {
        return triggerEvent;
    }
}
