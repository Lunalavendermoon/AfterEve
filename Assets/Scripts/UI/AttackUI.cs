using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;

public class AttackUI : MonoBehaviour
{

    //Singleton
    public static AttackUI Instance;
    public PlayerAttributes playerAttributes;


    [SerializeField] private GameObject ammoIconFolder;
    [SerializeField] private Image[] ammoIcons;

    [Header("Legacy Sprite Ammo UI")]
    [SerializeField] private bool useLegacyAmmoIcons = false;


    [SerializeField] private GameObject player; //for getting player's rotation
    [SerializeField] private Transform attackDirectionIndicator;
    [SerializeField] private Transform attackPivotCenter;


    [SerializeField] private Image reloadCircleImage;

    [Header("Ammo Counter")]
    [SerializeField] private TMP_Text ammoCounterText;

    private int totalAmmo;

    public int CurrentMaxAmmo => totalAmmo;



    void Awake()
    {
        Instance = GetComponent<AttackUI>();
        reloadCircleImage.fillAmount = 1f;

        RefreshAmmoIconsCache();

        // Sets pivote of attack direction indicator
        attackDirectionIndicator.transform.parent = attackPivotCenter;
    }

    // Every frame - update attack direction indicator rotation
    void Update()
    {
        //find the angle between the player position and the mouse position
        Vector2 mousePosition = Input.mousePosition;
        Vector2 playerScreenPosition = Camera.main.WorldToScreenPoint(player.transform.position);
        Vector2 targetAngleVector = mousePosition - playerScreenPosition;
        float targetAngle = Mathf.Atan2(targetAngleVector.y, targetAngleVector.x) * Mathf.Rad2Deg;

        attackPivotCenter.rotation = Quaternion.AngleAxis(targetAngle - 90, Vector3.forward);
    }



    // Ammo UI
    public void initializeAmmoUI()
    {
        RefreshAmmoUI();
    }

    private void RefreshAmmoUI()
    {
        RefreshAmmoIconsCache();

        // TMP counter should always update, even if legacy icons are disabled.
        if (!useLegacyAmmoIcons || ammoIconFolder == null)
        {
            if (ammoIconFolder != null) ammoIconFolder.SetActive(false);
            RefreshAmmoCounterText();
            return;
        }

        ammoIconFolder.SetActive(true);

        int childCount = ammoIconFolder.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform ammoIcon = ammoIconFolder.transform.GetChild(i);
            ammoIcon.gameObject.SetActive(i < totalAmmo);
        }

        // Reset colors for the icons that are currently in use.
        for (int i = 0; i < totalAmmo; i++)
        {
            if (ammoIcons == null || i >= ammoIcons.Length || ammoIcons[i] == null) continue;
            ammoIcons[i].color = new Color32(255, 255, 255, 255);
        }

        RefreshAmmoCounterText();
    }

    private void RefreshAmmoCounterText()
    {
        if (ammoCounterText == null) return;

        int current = 0;
        if (PlayerController.instance != null)
        {
            current = PlayerController.instance.CurrentAmmo;
        }

        int max = Mathf.Max(0, totalAmmo);
        ammoCounterText.text = $"{current}/{max}";
    }

    private void RefreshAmmoIconsCache()
    {
        PlayerAttributes attrs = PlayerController.instance != null ? PlayerController.instance.playerAttributes : playerAttributes;
        int desiredTotalAmmo = attrs != null ? attrs.Ammo : 0;
        desiredTotalAmmo = Mathf.Max(0, desiredTotalAmmo);

        // Always keep totalAmmo updated for TMP counter, even if legacy icons are disabled.
        if (!useLegacyAmmoIcons || ammoIconFolder == null)
        {
            totalAmmo = desiredTotalAmmo;
            ammoIcons = Array.Empty<Image>();
            return;
        }

        int childCount = ammoIconFolder.transform.childCount;
        int newTotal = Mathf.Min(desiredTotalAmmo, childCount);

        // Rebuild cache if size mismatches
        if (ammoIcons == null || ammoIcons.Length != newTotal)
        {
            ammoIcons = new Image[newTotal];
        }

        // Populate references to each ammo UI icon (only up to newTotal)
        for (int i = 0; i < newTotal; i++)
        {
            Transform ammoIcon = ammoIconFolder.transform.GetChild(i);
            ammoIcons[i] = ammoIcon.GetComponent<Image>();
        }

        totalAmmo = newTotal;
    }

    public void greyNextAmmo(int currentAmmo) //currentAmmo = remaining # of ammo
    {
        if (!useLegacyAmmoIcons)
        {
            RefreshAmmoCounterText();
            return;
        }

        PlayerAttributes attrs = PlayerController.instance != null ? PlayerController.instance.playerAttributes : playerAttributes;
        if (attrs != null && totalAmmo != Mathf.Max(0, attrs.Ammo))
        {
            RefreshAmmoIconsCache();
            RefreshAmmoUI();
        }

        int idx = totalAmmo - currentAmmo;
        if (idx < 0 || idx >= totalAmmo) return;
        if (ammoIcons == null || idx >= ammoIcons.Length || ammoIcons[idx] == null) return;
        ammoIcons[idx].color = new Color32(255, 255, 255, 25);

        // Refresh from the authoritative source to avoid off-by-one issues
        // (e.g. different call/decrement ordering or multi-bullet shots).
        RefreshAmmoCounterText();
    }

    public void resetAmmoUI()
    {
        if (!useLegacyAmmoIcons)
        {
            RefreshAmmoCounterText();
            return;
        }

        PlayerAttributes attrs = PlayerController.instance != null ? PlayerController.instance.playerAttributes : playerAttributes;
        if (attrs != null && totalAmmo != Mathf.Max(0, attrs.Ammo))
        {
            RefreshAmmoIconsCache();
            RefreshAmmoUI();
        }

        for (int i = 0; i < totalAmmo; i++)
        {
            if (ammoIcons == null || i >= ammoIcons.Length || ammoIcons[i] == null) continue;
            ammoIcons[i].color = new Color32(255, 255, 255, 255);
        }

        RefreshAmmoCounterText();
    }





    // Reload Animation
    public void runAmmoReloadAnimation()
    {
        StartCoroutine(reloadAnimationCoroutine());
    }

    private IEnumerator reloadAnimationCoroutine()
    {
        float reloadStartTime = Time.time;
        float reloadDuration = playerAttributes.reloadSpeed;
        //Debug.Log("reload duration: " + reloadDuration);

        reloadCircleImage.fillAmount = 0f;
        //Debug.Log("bar set to 0 - starting animation");

        while (Time.time < reloadStartTime + reloadDuration)
        {
            reloadCircleImage.fillAmount = (Time.time - reloadStartTime) / reloadDuration;
            yield return null;
        }
        reloadCircleImage.fillAmount = 1f;
    }
}
