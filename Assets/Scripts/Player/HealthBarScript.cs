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

    private PlayerController GetPlayer()
    {
        return PlayerController.instance;
    }

    // Set player's max health
    public void setMaxHitPoints(int health)
    {
        PlayerController player = GetPlayer();
        if (player == null)
        {
            return;
        }

        // UI-only: max HP should be sourced from the authoritative player health.
        slider.maxValue = player.MaxHealth;
        Debug.Log("Set max health to " + health);
        updateHitPointUIValues(player.CurrentHealth);
    }

    // Set player's current health
    public void setCurrentHitPoints(int health)
    {
        PlayerController player = GetPlayer();
        if (player == null)
        {
            return;
        }

        int newHealth = health;
        if(health > player.MaxHealth) {
            newHealth = player.MaxHealth;
        }
        if(health < 0) {
            newHealth = 0;
        }

        if(healthSlidingRoutine != null) StopCoroutine(healthSlidingRoutine);
        healthSlidingRoutine = StartCoroutine(healthSlidingAnimationCoroutine(newHealth));
    }

    public void SyncFromPlayer()
    {
        PlayerController player = GetPlayer();
        if (player == null) return;
        setMaxHitPoints(player.MaxHealth);
        setCurrentHitPoints(player.CurrentHealth);
    }

    public void updateHitPointUIValues(int displayedHitPoints) {
        PlayerController player = GetPlayer();
        int maxHp = player != null ? player.MaxHealth : 0;
        healthValueText.text = displayedHitPoints + " / " + maxHp;
    }

    // UI animation helper for smooth transition between diff health values
    private IEnumerator healthSlidingAnimationCoroutine(float finalHealth)
    {
        PlayerController player = GetPlayer();
        if (player == null)
        {
            yield break;
        }

        float startingHealth = slider.value;
        float slidingStartTime = Time.time;
        float slidingDuration = 0.01f * (Math.Abs(finalHealth - startingHealth))/Mathf.Max(1, player.MaxHealth) * 100; //0.05s sliding time/1% of health

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
