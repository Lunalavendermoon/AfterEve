using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Yarn;

public class DialogueHistoryLogUI : MonoBehaviour
{
    public GameObject logUI;
    public GameObject linePrefab;
    public GameObject lineContainer;
    public bool isOpen = false;

    public List<DialogueLine> historyLog = new List<DialogueLine>();

    public void AddLineToLog(string speaker, string text, bool choice)
    {
        DialogueLine newLine = new DialogueLine();
        newLine.speaker = speaker;
        newLine.text = text;
        newLine.isChoice = choice;

        historyLog.Add(newLine);
        AddToDisplayLog(newLine);
    }

    private void Update()
    {
        // placeholder for now
        if (Input.GetKeyUp(KeyCode.Escape)) 
        { 
            if (isOpen)
            {
                isOpen = false;
                logUI.SetActive(false);
            }    
            else
            {
                isOpen = true;
                logUI.SetActive(true); 
            }
        }
    }

    private void AddToDisplayLog(DialogueLine line)
    {
        GameObject entry = Instantiate(linePrefab, lineContainer.transform);
        TextMeshProUGUI[] text = entry.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (TextMeshProUGUI t in text)
        {
            if (t.gameObject.transform.GetSiblingIndex() == 0)
            {
                t.text = line.speaker;
            } 
            else
            {
                t.text = line.text;
            }
        }
    }
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public bool isChoice;
}
