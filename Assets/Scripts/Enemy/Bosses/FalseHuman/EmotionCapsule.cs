using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EmotionCapsule : MonoBehaviour
{
    bool isActivated = false;
    float timer = 5;

    void Start()
    {
        
    }

    private void Update()
    {
        if (!isActivated)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                BlowUp();
            }
        }
        else
        {
            isPlayerShielded();
            isWaveOver();
        }
        
    }

    private void isWaveOver()
    {
        throw new NotImplementedException();
    }

    private void isPlayerShielded()
    {
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < 2)
        {
            Debug.Log("Shielded");
        }
    }

    private void BlowUp()
    {

        PlayerController.instance.TakeDamage(600, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Spiritual);
        //apply effects
        Destroy(gameObject);
    }

    public void TakeDamage()
    {
        isActivated = true;
    }
}
