using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITestingScript : MonoBehaviour
{
    [SerializeField] private Button tarotCooldownTriggerButton;
    [SerializeField] private Button healHealthButton;
    [SerializeField] private Button damageHealthButton;
    [SerializeField] private Button addQuestProgressButton;
    [SerializeField] private Button loseQuestProgressButton;

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

        //tarotCooldownTriggerButton.onClick.AddListener(tarotUIScript.runTarotCooldownAnimation);
        healHealthButton.onClick.AddListener(() => PlayerController.instance.Heal(healAmount));
        damageHealthButton.onClick.AddListener(() => PlayerController.instance.TakeDamage(damageAmount, DamageInstance.DamageSource.Effect, DamageInstance.DamageType.Physical));
        addQuestProgressButton.onClick.AddListener(() => questUIScript.setQuestCurrentValue(questUIScript.getQuestCurrentValue() + 1));
        loseQuestProgressButton.onClick.AddListener(() => questUIScript.setQuestCurrentValue(questUIScript.getQuestCurrentValue() - 1));
    }
}
