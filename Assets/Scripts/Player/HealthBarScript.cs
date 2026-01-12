using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class HealthBarScript : MonoBehaviour
{
    // TODO: tie slider value to a health stat in PlayerAttributes if needed
    // - animation may be glitchy if you don't -> having separate health attr allows stopping other animations on new/frequent
    //   health change w/out losing changes to health

    public Slider slider;
    public float maxHealth;

    // temporary - for testing only
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            float newHealth = slider.value - 300;
            Debug.Log("h detected - setting new health to: " + newHealth);
            setHealth(newHealth);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            float newHealth = slider.value + 300;
            Debug.Log("j detected - setting new health to: " + newHealth);
            setHealth(newHealth);
        }
    }

    public void setMaxHealth(float health)
    {
        maxHealth = health;
        slider.maxValue = health;
        setHealth(health);
        Debug.Log("Set max health to " + health);
    }

    public void setHealth(float health)
    {
        float newHealth = health;
        if(health > maxHealth) {
            newHealth = maxHealth;
        }
        if(health < 0) {
            newHealth = 0;
        }
        StartCoroutine(healthSlidingAnimationCoroutine(newHealth));
    }

    // UI animation helper
    private IEnumerator healthSlidingAnimationCoroutine(float finalHealth)
    {
        float startingHealth = slider.value;
        float slidingStartTime = Time.time;
        float slidingDuration = 0.01f * (Math.Abs(finalHealth - startingHealth))/maxHealth * 100; //0.05s sliding time/1% of health

        while (Time.time - slidingStartTime < slidingDuration)
        {
            // new health at X point in time = startingHealth + proportion of total time passed * total health to change
            slider.value = startingHealth + ((Time.time - slidingStartTime)/slidingDuration) * (finalHealth - startingHealth);
            yield return null;
        }
        slider.value = finalHealth;
        yield break;
    }
}
