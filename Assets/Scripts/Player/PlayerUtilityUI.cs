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
        dashMaskImage.fillAmount = 1f;
        dashCooldownDuration = playerAttributes.dashCooldown;
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
