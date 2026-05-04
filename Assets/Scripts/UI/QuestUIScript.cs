using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class QuestUIScript : MonoBehaviour
{
    [SerializeField] private QuestUISlider sliderOne;
    [SerializeField] private QuestUISlider sliderTwo;
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;

    public void setQuestName(string questName) {
        questNameText.text = questName;
    }

    public void setQuestDescription(string questDescription) {
        questDescriptionText.text = questDescription;
    }

    public void setQuestSliderMaxValue(float newMaxValue, int sliderNum) 
    {
        if (sliderNum == 0)
        {
            if (sliderOne)
                sliderOne.setQuestMaxValue(newMaxValue);
        }
        else
        {
            if (sliderTwo)
                sliderTwo.setQuestMaxValue(newMaxValue);
        }
    }

    public void setQuestSliderCurrentValue(float newCurrentValue, int sliderNum) 
    {
        if (sliderNum == 0)
        {
            if (sliderOne)
                sliderOne.setQuestCurrentValue(newCurrentValue);
        }
        else
        {
            if (sliderTwo)
                sliderTwo.setQuestCurrentValue(newCurrentValue);
        }
    }

    public void destroyUponQuestCompletion()
    {
        if (this.gameObject)
            Destroy(this.gameObject);
    }

    public void disableSecondSlider()
    {
        if (sliderTwo)
            sliderTwo.gameObject.SetActive(false);
    }
}
