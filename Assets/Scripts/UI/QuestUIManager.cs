using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance;

    [SerializeField] private GameObject questContainer;
    [SerializeField] private QuestUIScript questUIPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public QuestUIScript SpawnQuestWithOneSlider()
    {
        GameObject questUI = Instantiate(questUIPrefab.gameObject, questContainer.transform);
        if (!questUI) return null;

        QuestUIScript questUIScript = questUI.GetComponent<QuestUIScript>();
        if (questUIScript)
        {
            questUIScript.disableSecondSlider();
            return questUIScript;
        }
        return null;
    }

    public QuestUIScript SpawnQuestWithTwoSliders()
    {
        GameObject questUI = Instantiate(questUIPrefab.gameObject, questContainer.transform);
        if (!questUI) return null;

        QuestUIScript questUIScript = questUI.GetComponent<QuestUIScript>();
        if (questUIScript)
            return questUIScript;

        return null;
    }
}
