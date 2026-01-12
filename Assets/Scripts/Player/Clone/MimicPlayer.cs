using System;
using UnityEngine;

public class MimicPlayer : MonoBehaviour
{
    public int detectionRadius;
    public Transform firingPoint;
    private float shootTime;
    public float lookAtEnemyDuration = 1.5f;
    public GameObject bombPrefab;
    private float lastTimeShot;
    private float lookTimer;
    private float fireRate = 5f;
    private bool waitingtoShoot;

    // Adjustable stats
    private int basicDamage;
    private int spiritualDamage;
    private float strengthBuff;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f; 

    private void Start()
    {
        lastTimeShot = Time.time;
        if (basicDamage == 0)
        {
            basicDamage = PlayerController.instance.playerAttributes.damage;
        }
        if (spiritualDamage == 0)
        {
            spiritualDamage = PlayerController.instance.playerAttributes.damage;
        }

        waitingtoShoot = false;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) > 2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerController.instance.transform.position, 2 * Time.deltaTime);
        }

        GameObject nearestEnemy = FindNearestEnemy();
        Vector3 targetPosition;

        if (nearestEnemy != null && Time.time - lastTimeShot > fireRate)
        {
            lastTimeShot = Time.time;
            lookTimer = Time.time + lookAtEnemyDuration;
            waitingtoShoot = true;
        }

        if (waitingtoShoot)
        {
            targetPosition = nearestEnemy.transform.position;
            if (Time.time > lookTimer)
            {
                Shoot(nearestEnemy);
                waitingtoShoot= false;
            }
        }
        else
        {
            targetPosition = PlayerController.instance.transform.position;
        }

        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Shoot(GameObject target)
    {
        GameObject bomb = Instantiate(bombPrefab, firingPoint.position, firingPoint.rotation);
        bomb.GetComponent<Bomb>().SetBasicDamage(basicDamage);
        bomb.GetComponent<Bomb>().SetSpiritualDamage(spiritualDamage);
        bomb.GetComponent<Bomb>().SetStrengthBuff(strengthBuff);
    }

    public void SetDamage(float a, float b, float c)
    {
        basicDamage = (int) (PlayerController.instance.playerAttributes.damage * a);
        spiritualDamage = (int)(PlayerController.instance.playerAttributes.damage * b);
        strengthBuff = c;
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= detectionRadius)
            {
                closestDistance = distance;
                nearest = enemy;
            }
        }
        return nearest;
    }
}