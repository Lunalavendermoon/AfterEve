using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class QuestUIScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private TMP_Text questValueText;
    private Coroutine questSlidingRoutine; //for preventing multiple animations

    private int maxValue;
    private int currentValue;

    public int getQuestMaxValue() {
        return maxValue;
    }
    public int getQuestCurrentValue() {
        return currentValue;
    }

    public void setQuestName(string questName) {
        questNameText.text = questName;
    }
    public void setQuestDescription(string questDescription) {
        questDescriptionText.text = questDescription;
    }

    public void setQuestMaxValue(int newMaxValue) {
        maxValue = newMaxValue;
        slider.maxValue = newMaxValue;

        currentValue = 0;
        slider.value = 0;

        questValueText.text = currentValue + "/" + maxValue;
    }
    public void setQuestCurrentValue(int newCurrentValue) {
        if(newCurrentValue < 0 || newCurrentValue > maxValue) {
            return;
        }
        currentValue = newCurrentValue;

        if(questSlidingRoutine != null) StopCoroutine(questSlidingRoutine);
        questSlidingRoutine = StartCoroutine(questSlidingAnimationCoroutine(currentValue));

        if(currentValue == maxValue) {
            questValueText.text = "Quest completed";
        }
    }



    // UI animation helper for smooth transition between diff health values
    private IEnumerator questSlidingAnimationCoroutine(float finalValue)
    {
        float startingValue = slider.value;
        float slidingStartTime = Time.time;
        float slidingDuration = 0.01f * (Math.Abs(finalValue - startingValue))/maxValue * 100; //0.01s sliding time/1% change of total health

        questValueText.text = finalValue + "/" + maxValue;

        while (Time.time - slidingStartTime < slidingDuration)
        {
            // new value at X point in time = startingValue + proportion of total time passed * total value to change
            slider.value = startingValue + ((Time.time - slidingStartTime)/slidingDuration) * (finalValue - startingValue);
            yield return null;
        }
        slider.value = finalValue;
        yield break;
    }
}
