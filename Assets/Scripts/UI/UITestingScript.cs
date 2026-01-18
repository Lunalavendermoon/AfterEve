using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITestingScript : MonoBehaviour
{
    [SerializeField] private Button tarotCooldownTriggerButton;
    [SerializeField] private Button healHealthButton;
    [SerializeField] private Button damageHealthButton;
    [SerializeField] private Button addQuestProgressButton;

    [SerializeField] private TMP_Text healButtonText;
    [SerializeField] private TMP_Text damageButtonText;

    [SerializeField] private int healAmount;
    [SerializeField] private int damageAmount;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private TarotUIScript tarotUIScript;

    private PlayerAttributes playerAttributes;
    private HealthBarScript healthBarScript;
    private QuestUIScript questUIScript;

    void Start()
    {
        playerAttributes = PlayerController.instance.playerAttributes;
        healthBarScript = playerController.healthBar;
        questUIScript = playerController.questUIScript;

        healButtonText.text = "Heal " + healAmount;
        damageButtonText.text = "Damage " + damageAmount;

        tarotCooldownTriggerButton.onClick.AddListener(tarotUIScript.runTarotCooldownAnimation);
        healHealthButton.onClick.AddListener(() => healthBarScript.setCurrentHitPoints(playerAttributes.currentHitPoints + healAmount));
        damageHealthButton.onClick.AddListener(() => healthBarScript.setCurrentHitPoints(playerAttributes.currentHitPoints - damageAmount));
        addQuestProgressButton.onClick.AddListener(() => questUIScript.setQuestCurrentValue(questUIScript.getQuestCurrentValue() + 1));
    }
}
