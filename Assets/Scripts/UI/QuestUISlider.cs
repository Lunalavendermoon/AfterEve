using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUISlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text questValueText;

    private Coroutine questSlidingRoutine; //for preventing multiple animations
    private float maxValue;
    private float currentValue;

    public float getQuestMaxValue()
    {
        return maxValue;
    }
    public float getQuestCurrentValue()
    {
        return currentValue;
    }

    public void setQuestMaxValue(float newMaxValue)
    {
        maxValue = newMaxValue;
        slider.maxValue = newMaxValue;

        currentValue = 0;
        slider.value = 0;

        questValueText.text = currentValue + " / " + maxValue;
    }

    public void setQuestCurrentValue(float newCurrentValue)
    {
        if (newCurrentValue < 0)
            return;

        if (newCurrentValue > maxValue)
            newCurrentValue = maxValue;

        if (maxValue <= 1)
            currentValue = (float)Math.Round(newCurrentValue, 2);
        else
            currentValue = (float)Math.Round(newCurrentValue, 0);

        if (questSlidingRoutine != null) StopCoroutine(questSlidingRoutine);
        questSlidingRoutine = StartCoroutine(questSlidingAnimationCoroutine(currentValue));

        if (currentValue == maxValue)
            questValueText.text = "Quest completed";
    }


    // UI animation helper for smooth transition between diff health values
    private IEnumerator questSlidingAnimationCoroutine(float finalValue)
    {
        float startingValue = slider.value;
        float slidingStartTime = Time.time;
        float slidingDuration = 0.01f * (Math.Abs(finalValue - startingValue)) / maxValue * 100; //0.01s sliding time/1% change of total health

        questValueText.text = finalValue + " / " + maxValue;

        while (Time.time - slidingStartTime < slidingDuration)
        {
            // new value at X point in time = startingValue + proportion of total time passed * total value to change
            slider.value = startingValue + ((Time.time - slidingStartTime) / slidingDuration) * (finalValue - startingValue);
            yield return null;
        }
        slider.value = finalValue;
        yield break;
    }
}
