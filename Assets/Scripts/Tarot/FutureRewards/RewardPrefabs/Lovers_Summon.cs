using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lovers_Summon : MonoBehaviour
{
    private float lastTimeShot = 0;
    public GameObject projectilePrefab;
    private void Update()
    {
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) > 2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerController.instance.transform.position, 2 * Time.deltaTime);
        }
        if (PlayerController.instance.playerInput.Player.Attack.triggered)
        {
            float firingSpeed = 1 / PlayerController.instance.playerAttributes.attackPerSec;
            if (lastTimeShot + firingSpeed <= Time.time && !PlayerController.instance.currentlyReloading)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f;
                Vector3 direction = (mouseWorldPos - transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);
                GameObject projectile = Instantiate(projectilePrefab, transform.position, rotation);
                int damage = (int)(PlayerController.instance.playerAttributes.damage * 0.4);
                projectile.GetComponent<Projectile>().SetPhysicalDamage(damage);
                lastTimeShot = Time.time;
            }
        }
    }
}