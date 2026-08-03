using UnityEngine;

public class Weakpoints : MonoBehaviour
{
    [SerializeField] private float cooldownAfterHit = 5f;
    GameObject weakpoint;
    private bool inCooldown;
    private float cooldownEndTime;

    private void Start()
    {
        PlayerController.OnSpiritualVisionChange += OnSpiritualVisionChanged;
        updateWeakPointPosition();
        weakpoint = transform.GetChild(0).gameObject;
        // Hide weakpoint initially
        weakpoint.SetActive(false);
    }

    private void OnEnable()
    {
        Projectile.OnWeakpointHit += OnWeakpointHit;
    }

    private void OnDisable()
    {
        PlayerController.OnSpiritualVisionChange -= OnSpiritualVisionChanged;
        Projectile.OnWeakpointHit -= OnWeakpointHit;
    }

    private void Update()
    {
        if (inCooldown && Time.time >= cooldownEndTime)
        {
            inCooldown = false;
            weakpoint.SetActive(false);  // Return to spiritual vision control
        }

        if (Time.frameCount % 500 == 0)
        {
            updateWeakPointPosition();
        }
    }

    private void OnSpiritualVisionChanged(bool isInSpiritualVision)
    {
        // Show weakpoint only when player is in spiritual vision (and not in cooldown)
        if (!inCooldown)
        {
            weakpoint.SetActive(isInSpiritualVision);
        }
    }

    private void OnWeakpointHit(Transform hitWeakpoint)
    {
        // Only handle if this is the weakpoint that was hit
        if (hitWeakpoint == transform)
        {
            inCooldown = true;
            cooldownEndTime = Time.time + cooldownAfterHit;
            weakpoint.SetActive(false);
            // Notify enemy if it implements the interface
            IKnightWithWeakPoint knightWithWeakpoint = GetComponentInParent<IKnightWithWeakPoint>();
            if (knightWithWeakpoint != null)
            {
                knightWithWeakpoint.NotifyWeakPointHitBySpiritual();
            }
            else
            {
                Debug.LogWarning("Parent does not implement IKnightWithWeakPoint interface.");
            }
        }
    }

    public void updateWeakPointPosition()
    {
        //roate the gameobject randomly on the z axis
        transform.Rotate(0, 0, Random.Range(0, 360));
    }
}
