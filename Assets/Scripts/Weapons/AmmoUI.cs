using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public PlayerAttributes playerAttributes;
    [SerializeField] private Image[] bulletIcons;

    [SerializeField] private GameObject player;
    [SerializeField] private Transform attackDirectionIndicator;
    [SerializeField] private Transform attackPivotCenter;

    //Singleton
    public static AmmoUI Instance;

    private int totalAmmo;

    void Awake()
    {
        Instance = GetComponent<AmmoUI>();
        totalAmmo = playerAttributes.Ammo;
        //initializeAmmoUI(3); //Testing
    }
    
    public float radius = 2f;

    void Start()
    {
        attackDirectionIndicator.transform.parent = attackPivotCenter;
        attackDirectionIndicator.transform.position += Vector3.up * radius;
    }

    void Update()
    {
        // get rotation direction (as vector)
        Vector3 targetAngleVector = Input.mousePosition - Camera.main.WorldToScreenPoint(player.transform.position);
        float targetAngle = Mathf.Atan2(targetAngleVector.y, targetAngleVector.x) * Mathf.Rad2Deg;

        // pivot.position = orb.position; //not necessary (?)
        attackPivotCenter.rotation = Quaternion.AngleAxis(targetAngle - 90, Vector3.forward);
    }

    // void Update()
    // {
    //     Vector3 mouse_pos = Input.mousePosition;
	//     Vector3 player_pos = Camera.main.WorldToScreenPoint(player.position);
	//     mouse_pos.x = mouse_pos.x - player_pos.x;
	//     mouse_pos.y = mouse_pos.y - player_pos.y;
	    
    //     float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
    //     Vector3 targetAngle = new Vector3(0, 0, angle);


    //     // Quaternion targetAngle = Quaternion.Euler(new Vector3(0, 0, angle));
    //     Debug.Log("Rotation angle: " + targetAngle);
    //     // transform.RotateAround(target.transform.position, Vector3.up, 20 * Time.deltaTime);

    //     transform.RotateAround(attackPivotCenter.position, Vector3.up, 20 * Time.deltaTime);
    // }

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
