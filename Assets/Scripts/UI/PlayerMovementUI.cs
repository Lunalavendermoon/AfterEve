using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// named kind of badly: Movement-Related UI;
public class PlayerMovementUI : MonoBehaviour
{
    // Singleton
    public static PlayerMovementUI Instance;

    // for dash animation
    [SerializeField] private Image dashMaskImage;
    private float dashCooldownDuration;

    // for movement direction indicator ui
    [SerializeField] private RectTransform movePad; // background
    [SerializeField] private RectTransform origin; // exists bc fsr 0, 0 is offset a little
    [SerializeField] private RectTransform joystickCircle;
    private float deadZone = 0.05f;
    private Coroutine moveRoutine;

    void Awake()
    {
        Instance = GetComponent<PlayerMovementUI>();
        dashMaskImage.fillAmount = 1f;
    }

    void Start()
    {
        dashCooldownDuration = PlayerController.instance.playerAttributes.dashCooldown;
    }


    // Dash refresh animation
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


    // Movement direction indicator UI
    public void SetMoveUIDirection(float horizontalInput, float verticalInput)
    {
        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput);
        float radius = movePad.rect.width * 0.5f - movePad.rect.width * 0.1f;
        movementDirection.Normalize(); // performed in-place
        Vector2 targetPosition = movementDirection * radius;

        // if movement vector length is basically 0 -> reset position
        if (movementDirection.sqrMagnitude < deadZone * deadZone)
        {
            targetPosition = origin.anchoredPosition;
        }
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(moveUIAnimation(targetPosition));
    }

    private IEnumerator moveUIAnimation(Vector2 targetPosition) {
        while ((joystickCircle.anchoredPosition - targetPosition).sqrMagnitude > 0.1f)
        {
            joystickCircle.anchoredPosition = Vector2.Lerp(joystickCircle.anchoredPosition, targetPosition, Time.unscaledDeltaTime * 10f);
            yield return null;
        }

        joystickCircle.anchoredPosition = targetPosition;
        moveRoutine = null;
    }
}
