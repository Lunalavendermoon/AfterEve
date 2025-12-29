using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUtilityUI : MonoBehaviour
{
    //Singleton
    public static PlayerUtilityUI Instance;

    public PlayerAttributes playerAttributes;
    [SerializeField] private Image dashMaskImage;

    private float dashCooldownDuration;

    void Awake()
    {
        Instance = GetComponent<PlayerUtilityUI>();
        // dashCooldownDuration = playerAttributes.dashCooldown;

        dashMaskImage.fillAmount = 1f;
        dashCooldownDuration = playerAttributes.dashCooldown;

        // 1. is dash tied to space bar - ui says shifts
        // 2. is dash cooldown implemented anywhere (or is the dash duration just the dash cooldown by default)
            // is there no cooldown before dashes - the cooldown just shows the duration of the dash?

        // I'm not sure if I broke smth but right now u can dash again consecutively/during the dash
    }

    public void triggerDashUsedUI()
    {
        dashMaskImage.fillAmount = 0f;
        dashRefreshAnimation();
    }

    public void dashRefreshAnimation()
    {
        StartCoroutine(dashRefreshAnimationCoroutine());
    }

    private IEnumerator dashRefreshAnimationCoroutine()
    {
        float cooldownStartTime = Time.time;
        dashMaskImage.fillAmount = 0f;

        while (Time.time < cooldownStartTime + dashCooldownDuration)
        {
            dashMaskImage.fillAmount = (Time.time - cooldownStartTime) / dashCooldownDuration;
            yield return null;
        }
        dashMaskImage.fillAmount = 1f;
        yield break;
    }
}
