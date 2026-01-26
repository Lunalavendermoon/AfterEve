using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEditor;

[RequireComponent(typeof(Image))]
public class TarotUIScript : MonoBehaviour
{

    [SerializeField] float tarotCooldown;
    //[SerializeField] private TMP_Text tarotCooldownButtonText;
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
        //(tarotCooldownButtonText != null) tarotCooldownButtonText.text = "[in cooldown]";

        float cooldownStartTime = Time.time;
        float cooldownDuration = tarotCooldown;
        
        // create greyed out card as child
        GameObject obj = Instantiate(gameObject, transform);
        obj.transform.position = transform.position;
        Destroy(obj.GetComponent<TarotUIScript>());
        Image cooldownFill = obj.GetComponent<Image>();
        cooldownFill.type = Image.Type.Filled;
        cooldownFill.sprite = GetComponent<Image>().sprite;
        cooldownFill.color = Color.grey;

        cooldownFill.fillAmount = 1f;
        cooldownFill.fillAmount = 0.5f;

        while (Time.time < cooldownStartTime + cooldownDuration)
        {
            cooldownFill.fillAmount = 1 - ((Time.time - cooldownStartTime) / cooldownDuration);
            yield return null;
        }
        
        Destroy(obj);

        tarotCooldownRoutine = null;

        //testing
        //if(tarotCooldownButtonText != null) tarotCooldownButtonText.text = "Use Tarot";
    }
}
