using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{

    public Slider slider;

    public void setMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        Debug.Log("Set values to " + health);
    }
    public void setHealth(int health)
    {
        slider.value = health;
    }
}
