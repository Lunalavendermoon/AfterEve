using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TarotUIScript : MonoBehaviour
{

    [SerializeField] float tarotCooldown;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TMP_Text tarotCooldownButtonText;
    private Coroutine tarotCooldownRoutine;

    // Tarot Cooldown Animation
    public void runTarotCooldownAnimation()
    {
        if(tarotCooldownRoutine != null) {
            Debug.Log("Tarot in cooldown -- cannot use at this time");
            return;
        }
        tarotCooldownRoutine = StartCoroutine(tarotCooldownAnimationCoroutine());
    }

    private IEnumerator tarotCooldownAnimationCoroutine()
    {
        //testing
        if(tarotCooldownButtonText != null) tarotCooldownButtonText.text = "[in cooldown]";


        float cooldownStartTime = Time.time;
        float cooldownDuration = tarotCooldown;

        cooldownFill.fillAmount = 1f;

        while (Time.time < cooldownStartTime + cooldownDuration)
        {
            cooldownFill.fillAmount = 1 - ((Time.time - cooldownStartTime) / cooldownDuration);
            yield return null;
        }
        cooldownFill.fillAmount = 0f;
        tarotCooldownRoutine = null;

        //testing
        if(tarotCooldownButtonText != null) tarotCooldownButtonText.text = "Use Tarot";
    }
}
