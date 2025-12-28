using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public PlayerAttributes playerAttributes;
    [SerializeField] private Image[] bulletIcons;

    //Singleton
    public static AmmoUI Instance;

    private int totalAmmo;

    void Awake()
    {
        Instance = GetComponent<AmmoUI>();
        totalAmmo = playerAttributes.Ammo;
        //initializeAmmoUI(3); //Testing
    }

    public void initializeAmmoUI()
    {
        for(int i = 0; i < totalAmmo; i++)
        {
            bulletIcons[i].gameObject.SetActive(true);
            //Debug.Log("activated bullet #: " + i);
        } 
    }

    public void greyNextAmmo(int currentAmmo) //currentAmmo = remaining # of ammo
    {
        bulletIcons[totalAmmo - currentAmmo].GetComponent<Image>().color = new Color32(255, 255, 255, 25);
        /*
         * 5 ammo
         * 2 ammo remaining (indexes 0, 1, 2 grayed)
         * 
         * on fire - gray 3
         */
    }

    public void resetAmmoUI()
    {
        for (int i = 0; i < totalAmmo; i++)
        {
            bulletIcons[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }
}
