using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AttackUI : MonoBehaviour
{

    //Singleton
    public static AttackUI Instance;
    public PlayerAttributes playerAttributes;


    [SerializeField] private GameObject ammoIconFolder;
    [SerializeField] private Image[] ammoIcons;


    [SerializeField] private GameObject player; //for getting player's rotation
    [SerializeField] private Transform attackDirectionIndicator;
    [SerializeField] private Transform attackPivotCenter;


    [SerializeField] private Image reloadCircleImage;

    private int totalAmmo;



    void Awake()
    {
        Instance = GetComponent<AttackUI>();
        reloadCircleImage.fillAmount = 1f;

        // Populate references to each ammo UI icon
        totalAmmo = playerAttributes.Ammo;
        foreach(Transform ammoIcon in ammoIconFolder.transform)
        {
            ammoIcons[ammoIcon.GetSiblingIndex()] = ammoIcon.GetComponent<Image>();
            ammoIcon.gameObject.SetActive(false);
        }

        // Sets pivote of attack direction indicator
        attackDirectionIndicator.transform.parent = attackPivotCenter;
    }

    // Every frame - update attack direction indicator rotation
    void Update()
    {
        Vector3 targetAngleVector = Input.mousePosition - Camera.main.WorldToScreenPoint(player.transform.position);
        float targetAngle = Mathf.Atan2(targetAngleVector.y, targetAngleVector.x) * Mathf.Rad2Deg;

        attackPivotCenter.rotation = Quaternion.AngleAxis(targetAngle - 90, Vector3.forward);
    }



    /*Ammo UI Functions*/
    public void initializeAmmoUI()
    {
        // TODO: if needed - update to dynamically create ammo icons based on weapon type (?)
        for(int i = 0; i < totalAmmo; i++)
        {
            ammoIcons[i].gameObject.SetActive(true);
        } 
    }

    public void greyNextAmmo(int currentAmmo) //currentAmmo = remaining # of ammo
    {
        ammoIcons[totalAmmo - currentAmmo].GetComponent<Image>().color = new Color32(255, 255, 255, 25);
    }

    public void resetAmmoUI()
    {
        for (int i = 0; i < totalAmmo; i++)
        {
            ammoIcons[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }





    /*Reload Animation Functions*/
    public void runAmmoReloadAnimation()
    {
        StartCoroutine(reloadAnimationCoroutine());
    }

    private IEnumerator reloadAnimationCoroutine()
    {
        float reloadStartTime = Time.time;
        float reloadDuration = playerAttributes.reloadSpeed;
        Debug.Log("reload duration: " + reloadDuration);

        reloadCircleImage.fillAmount = 0f;
        Debug.Log("bar set to 0 - starting animation");

        while (Time.time < reloadStartTime + reloadDuration)
        {
            reloadCircleImage.fillAmount = (Time.time - reloadStartTime) / reloadDuration;
            yield return null;
        }
        reloadCircleImage.fillAmount = 1f;
    }
}
