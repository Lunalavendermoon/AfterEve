using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;
    private Coroutine healthSlidingRoutine; //for preventing multiple animations
    [SerializeField] private TMP_Text healthValueText;
    private PlayerAttributes playerAttributes;

    private void Start() {
        playerAttributes = PlayerController.instance.playerAttributes;
    }

    // Set player's max health
    public void setMaxHitPoints(int health)
    {
        playerAttributes.maxHitPoints = health;
        slider.maxValue = health;
        Debug.Log("Set max health to " + health);
        updateHitPointUIValues(playerAttributes.currentHitPoints);
    }

    // Set player's current health
    public void setCurrentHitPoints(int health)
    {
        int newHealth = health;
        if(health > playerAttributes.maxHitPoints) {
            newHealth = playerAttributes.maxHitPoints;
        }
        if(health < 0) {
            newHealth = 0;
        }

        playerAttributes.currentHitPoints = newHealth;

        if(healthSlidingRoutine != null) StopCoroutine(healthSlidingRoutine);
        healthSlidingRoutine = StartCoroutine(healthSlidingAnimationCoroutine(newHealth));
    }

    public void updateHitPointUIValues(int currentHitPoints) {
        healthValueText.text = currentHitPoints + "/" + playerAttributes.maxHitPoints;
    }

    // UI animation helper for smooth transition between diff health values
    private IEnumerator healthSlidingAnimationCoroutine(float finalHealth)
    {
        float startingHealth = slider.value;
        float slidingStartTime = Time.time;
        float slidingDuration = 0.01f * (Math.Abs(finalHealth - startingHealth))/playerAttributes.maxHitPoints * 100; //0.05s sliding time/1% of health

        while (Time.time - slidingStartTime < slidingDuration)
        {
            // new health at X point in time = startingHealth + proportion of total time passed * total health to change
            slider.value = startingHealth + ((Time.time - slidingStartTime)/slidingDuration) * (finalHealth - startingHealth);
            updateHitPointUIValues((int)slider.value);
            yield return null;
        }
        
        updateHitPointUIValues((int)finalHealth);
        slider.value = finalHealth;
        yield break;
    }
}
