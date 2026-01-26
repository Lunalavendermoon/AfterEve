using UnityEngine;

public class TakeDamageScript : MonoBehaviour
{
    public EnemyBase enemyBase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Taking Damage");
            enemyBase.TakeDamage(10, DamageInstance.DamageSource.Player, DamageInstance.DamageType.Physical);
        }
    }
}
